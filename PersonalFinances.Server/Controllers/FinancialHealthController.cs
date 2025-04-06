using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Analytics;
using PersonalFinances.BLL.Interfaces.Analytics;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class FinancialHealthController : ControllerBase
    {
        private readonly IFinancialHealthService _service;

        public FinancialHealthController(IFinancialHealthService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetFinancialHealth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var healthScore = await _service.GetLatestFinancialHealthAsync(userId);

            if (healthScore == null)
            {
                healthScore = await _service.CalculateFinancialHealthAsync(userId);
            }

            return Ok(APIResponse<FinancialHealthModel>.SuccessResponse(healthScore, "Pontuação de saúde financeira calculada com sucesso."));
        }

        [HttpGet("recalculate")]
        public async Task<IActionResult> RecalculateFinancialHealth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var healthScore = await _service.CalculateFinancialHealthAsync(userId);

            return Ok(APIResponse<FinancialHealthModel>.SuccessResponse(healthScore, "Pontuação de saúde financeira recalculada com sucesso."));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetFinancialHealthHistory([FromQuery] int months = 6)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var history = await _service.GetFinancialHealthHistoryAsync(userId, months);

            return Ok(APIResponse<IEnumerable<FinancialHealthModel>>.SuccessResponse(history, "Histórico de saúde financeira obtido com sucesso."));
        }
    }
}