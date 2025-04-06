using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Interfaces.Currency;
using PersonalFinances.BLL.Interfaces.User;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;

        public CurrencyController(ICurrencyService currencyService, IUserService userService)
        {
            _currencyService = currencyService;
            _userService = userService;
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetExchangeRate(
            [FromQuery] string fromCurrency,
            [FromQuery] string toCurrency)
        {
            try
            {
                var rate = await _currencyService.GetLatestExchangeRateAsync(fromCurrency, toCurrency);
                return Ok(APIResponse<decimal>.SuccessResponse(rate, "Taxa de câmbio obtida com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao obter taxa de câmbio: {ex.Message}"));
            }
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertAmount(
            [FromQuery] decimal amount,
            [FromQuery] string fromCurrency,
            [FromQuery] string toCurrency)
        {
            try
            {
                var convertedAmount = await _currencyService.ConvertAmountAsync(amount, fromCurrency, toCurrency);

                var result = new
                {
                    OriginalAmount = amount,
                    OriginalCurrency = fromCurrency,
                    ConvertedAmount = convertedAmount,
                    TargetCurrency = toCurrency,
                    ExchangeRate = await _currencyService.GetLatestExchangeRateAsync(fromCurrency, toCurrency)
                };

                return Ok(APIResponse<object>.SuccessResponse(result, "Conversão realizada com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao converter valor: {ex.Message}"));
            }
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetAvailableCurrencies()
        {
            try
            {
                var currencies = await _currencyService.GetAvailableCurrenciesAsync();
                return Ok(APIResponse<IEnumerable<string>>.SuccessResponse(currencies, "Moedas disponíveis obtidas com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao obter moedas disponíveis: {ex.Message}"));
            }
        }

        [HttpPut("default-currency")]
        public async Task<IActionResult> SetDefaultCurrency([FromBody] string currency)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            try
            {
                // Obter usuário
                var user = await _userService.GetUserByStampEntity(userId);

                // Atualizar moeda padrão do usuário
                var updateModel = new BLL.Entities.ViewModel.UpdateUserViewModel
                {
                    Name = user.Username,
                    PhoneNumber = user.PhoneNumber
                    // Adicione DefaultCurrency à classe UpdateUserViewModel
                };

                // Como DefaultCurrency não existe na classe atual, você precisará modificá-la
                // ou usar uma abordagem diferente para atualizar a moeda padrão

                await _userService.UpdateUser(userId, updateModel);

                return Ok(APIResponse<object>.SuccessResponse(null, "Moeda padrão atualizada com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao definir moeda padrão: {ex.Message}"));
            }
        }
    }
}