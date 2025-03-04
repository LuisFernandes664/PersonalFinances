using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Notification;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.BLL.Interfaces.Transaction;

namespace PersonalFinances.Server.Controllers
{

    namespace PersonalFinances.API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize(Policy = "Bearer")]
        public class TransactionsController : ControllerBase
        {
            private readonly ITransactionService _service;
            private readonly IGoalService _goalService;
            private readonly IBudgetService _budgetService;

            public TransactionsController(ITransactionService service)
            {
                _service = service;
            }

            #region Geral

            [HttpGet]
            public async Task<IActionResult> GetTransactions()
            {
                var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userStamp))
                    return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

                var transactions = await _service.GetTransactionsAsync(userStamp);
                return Ok(APIResponse<IEnumerable<TransactionModel>>.SuccessResponse(transactions, "Transações obtidas com sucesso."));
            }

            [HttpGet("{stampEntity}")]
            public async Task<IActionResult> GetByStampEntity(string stampEntity)
            {
                var transaction = await _service.GetTransactionByStampEntityAsync(stampEntity);
                if (transaction == null)
                    return NotFound(APIResponse<TransactionModel>.FailResponse("Transacção não encontrada."));

                var response = APIResponse<TransactionModel>.SuccessResponse(transaction, "Transacção obtida com sucesso.");
                return Ok(response);
            }

            [HttpPost]
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] TransactionModel transaction)
            {
                var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userStamp))
                    return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

                transaction.UserStamp = userStamp;

                await _service.AddTransactionAsync(transaction);

                // Atualiza Budget ou Goal automaticamente após criar a transação
                if (!string.IsNullOrEmpty(transaction.ReferenceId))
                {
                    if (transaction.ReferenceType == "Budget")
                    {
                        await _budgetService.UpdateBudgetSpentAmount(transaction.ReferenceId);
                    }
                    else if (transaction.ReferenceType == "Goal")
                    {
                        await _goalService.UpdateGoalAccumulatedAmount(transaction.ReferenceId);
                    }
                }

                return Ok(APIResponse<TransactionModel>.SuccessResponse(transaction, "Transação criada com sucesso."));
            }


            [HttpPut("{stampEntity}")]
            public async Task<IActionResult> Update(string stampEntity, [FromBody] TransactionModel transaction)
            {
                if (!ModelState.IsValid)
                    return BadRequest(APIResponse<TransactionModel>.FailResponse(ModelState));

                try
                {
                    await _service.UpdateTransactionAsync(stampEntity, transaction);
                    var response = APIResponse<object>.SuccessResponse(null, "Transacção atualizada com sucesso.");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return NotFound(APIResponse<TransactionModel>.FailResponse(ex.Message));
                }
            }

            [HttpDelete("{stampEntity}")]
            public async Task<IActionResult> Delete(string stampEntity)
            {
                await _service.DeleteTransactionAsync(stampEntity);
                var response = APIResponse<object>.SuccessResponse(null, "Transacção eliminada com sucesso.");
                return Ok(response);
            }

            #endregion

            #region Structural Objects

            [HttpGet("totals")]
            public async Task<IActionResult> GetTotals()
            {
                var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userStamp))
                    return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

                var totalBalance = await _service.GetTotalBalanceAsync(userStamp);
                var totalIncome = await _service.GetTotalIncomeAsync(userStamp);
                var totalExpenses = await _service.GetTotalExpensesAsync(userStamp);

                var totals = new { totalBalance, totalIncome, totalExpenses };
                var response = APIResponse<object>.SuccessResponse(totals, "Totais calculados com sucesso.");
                return Ok(response);
            }

            [HttpGet("dashboard-totals")]
            public async Task<IActionResult> GetDashboardTotals()
            {
                try
                {
                    var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userStamp))
                        return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

                    var totals = await _service.GetDashboardTotalsAsync(userStamp);
                    var response = APIResponse<DashboardTotalsModel>.SuccessResponse(totals, "Dashboard totals calculated successfully.");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(APIResponse<object>.FailResponse($"Error calculating dashboard totals: {ex.Message}"));
                }
            }

            [HttpGet("chartdata")]
            public async Task<IActionResult> GetChartData([FromQuery] string interval)
            {
                try
                {
                    var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userStamp))
                        return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

                    var chartData = await _service.GetChartDataAsync(interval, userStamp);
                    var responseData = new
                    {
                        series = chartData.Series,
                        categories = chartData.Categories
                    };
                    var response = APIResponse<object>.SuccessResponse(responseData, "Dados do gráfico obtidos com sucesso.");
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(APIResponse<object>.FailResponse($"Erro ao obter dados do gráfico: {ex.Message}"));
                }
            }

            #endregion
        }
    }

}
