using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class RecurringTransactionsController : ControllerBase
    {
        private readonly IRecurringTransactionService _service;

        public RecurringTransactionsController(IRecurringTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecurringTransactions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<RecurringTransactionModel>.FailResponse("Utilizador não autenticado."));

            var transactions = await _service.GetUserRecurringTransactionsAsync(userId);
            return Ok(APIResponse<IEnumerable<RecurringTransactionModel>>.SuccessResponse(transactions, "Transações recorrentes obtidas com sucesso."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecurringTransaction(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var transaction = await _service.GetRecurringTransactionAsync(id);

            if (transaction == null)
                return NotFound(APIResponse<RecurringTransactionModel>.FailResponse("Transação recorrente não encontrada."));

            if (transaction.UserId != userId)
                return Forbid();

            return Ok(APIResponse<RecurringTransactionModel>.SuccessResponse(transaction, "Transação recorrente obtida com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecurringTransaction([FromBody] RecurringTransactionModel transaction)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<RecurringTransactionModel>.FailResponse("Utilizador não autenticado."));

            transaction.UserId = userId;
            await _service.CreateRecurringTransactionAsync(transaction);

            return Ok(APIResponse<RecurringTransactionModel>.SuccessResponse(transaction, "Transação recorrente criada com sucesso."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecurringTransaction(string id, [FromBody] RecurringTransactionModel transaction)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTransaction = await _service.GetRecurringTransactionAsync(id);

            if (existingTransaction == null)
                return NotFound(APIResponse<RecurringTransactionModel>.FailResponse("Transação recorrente não encontrada."));

            if (existingTransaction.UserId != userId)
                return Forbid();

            transaction.StampEntity = id;
            transaction.UserId = userId;

            await _service.UpdateRecurringTransactionAsync(transaction);
            return Ok(APIResponse<RecurringTransactionModel>.SuccessResponse(transaction, "Transação recorrente atualizada com sucesso."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecurringTransaction(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingTransaction = await _service.GetRecurringTransactionAsync(id);

            if (existingTransaction == null)
                return NotFound(APIResponse<RecurringTransactionModel>.FailResponse("Transação recorrente não encontrada."));

            if (existingTransaction.UserId != userId)
                return Forbid();

            await _service.DeleteRecurringTransactionAsync(id);
            return Ok(APIResponse<object>.SuccessResponse(null, "Transação recorrente removida com sucesso."));
        }
    }
}