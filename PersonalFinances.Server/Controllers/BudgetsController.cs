using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;

namespace PersonalFinances.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
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
    }

}
