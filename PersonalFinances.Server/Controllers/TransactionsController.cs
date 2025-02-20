using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;

namespace PersonalFinances.Server.Controllers
{

    namespace PersonalFinances.API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class TransactionsController : ControllerBase
        {
            private readonly ITransactionService _service;

            public TransactionsController(ITransactionService service)
            {
                _service = service;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var transactions = await _service.GetTransactionsAsync();
                var response = APIResponse<IEnumerable<TransactionModel>>.SuccessResponse(transactions, "Transacções obtidas com sucesso.");
                return Ok(response);
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
            public async Task<IActionResult> Create([FromBody] TransactionModel transaction)
            {
                if (!ModelState.IsValid)
                    return BadRequest(APIResponse<TransactionModel>.FailResponse(ModelState));

                await _service.AddTransactionAsync(transaction);
                var response = APIResponse<TransactionModel>.SuccessResponse(transaction, "Transacção criada com sucesso.");
                return CreatedAtAction(nameof(GetByStampEntity), new { stampEntity = transaction.StampEntity }, response);
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

            [HttpGet("totals")]
            public async Task<IActionResult> GetTotals()
            {
                var totalBalance = await _service.GetTotalBalanceAsync();
                var totalIncome = await _service.GetTotalIncomeAsync();
                var totalExpenses = await _service.GetTotalExpensesAsync();

                var totals = new { totalBalance, totalIncome, totalExpenses };
                var response = APIResponse<object>.SuccessResponse(totals, "Totais calculados com sucesso.");
                return Ok(response);
            }

            [HttpGet("dashboard-totals")]
            public async Task<IActionResult> GetDashboardTotals()
            {
                try
                {
                    var totals = await _service.GetDashboardTotalsAsync();
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
                    var chartData = await _service.GetChartDataAsync(interval);
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

        }
    }

}
