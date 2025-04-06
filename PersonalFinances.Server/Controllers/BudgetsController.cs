using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Analytics;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using System.Security.Claims;

namespace PersonalFinances.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _service;
        public BudgetsController(IBudgetService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBudgetsByUser(string userId)
        {
            var budgets = await _service.GetBudgetsByUserAsync(userId);
            return Ok(APIResponse<IEnumerable<BudgetModel>>.SuccessResponse(budgets, "Orçamentos obtidos com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetModel budget)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<BudgetModel>.FailResponse(ModelState));

            var userStamp = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userStamp))
                return Unauthorized(APIResponse<TransactionModel>.FailResponse("Utilizador não autenticado."));

            budget.UserId = userStamp;
            await _service.CreateBudgetAsync(budget);
            return Ok(APIResponse<BudgetModel>.SuccessResponse(budget, "Orçamento criado com sucesso."));
        }

        [HttpPut("{stampEntity}")]
        public async Task<IActionResult> UpdateBudget(string stampEntity, [FromBody] BudgetModel budget)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<BudgetModel>.FailResponse(ModelState));

            budget.StampEntity = stampEntity;
            await _service.UpdateBudgetAsync(budget);
            return Ok(APIResponse<BudgetModel>.SuccessResponse(budget, "Orçamento atualizado com sucesso."));
        }

        [HttpDelete("{stampEntity}")]
        public async Task<IActionResult> DeleteBudget(string stampEntity)
        {
            await _service.DeleteBudgetAsync(stampEntity);
            return Ok(APIResponse<object>.SuccessResponse(null, "Orçamento removido com sucesso."));
        }

        [HttpGet("{budgetId}/analytics")]
        public async Task<IActionResult> GetBudgetAnalytics(string budgetId)
        {
            var analytics = await _service.GetBudgetAnalyticsAsync(budgetId);
            return Ok(APIResponse<BudgetAnalyticsModel>.SuccessResponse(analytics, "Análise de orçamento obtida com sucesso."));
        }

        [HttpGet("{budgetId}/forecast")]
        public async Task<IActionResult> GetBudgetForecast(string budgetId, [FromQuery] int months = 3)
        {
            var forecast = await _service.GetBudgetForecastAsync(budgetId, months);
            return Ok(APIResponse<BudgetForecastModel>.SuccessResponse(forecast, "Previsão de orçamento calculada com sucesso."));
        }
    }

}
