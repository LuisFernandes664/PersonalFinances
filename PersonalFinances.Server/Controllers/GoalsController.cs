using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using System.Security.Claims;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _service;

        public GoalsController(IGoalService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtém todas as metas do utilizador autenticado (usando JWT).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGoalsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<GoalModel>.FailResponse("Utilizador não autenticado."));


            var goals = await _service.GetGoalsByUserAsync(userId);
            return Ok(APIResponse<IEnumerable<GoalModel>>.SuccessResponse(goals, "Metas obtidas com sucesso."));
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _service.GetCategoriesAsync() ?? new List<System.Web.Mvc.SelectListItem>();

            return Ok(APIResponse<IEnumerable<System.Web.Mvc.SelectListItem>>.SuccessResponse(categories, "Categorias obtidas com sucesso."));
        }


        /// <summary>
        /// Cria uma nova meta.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] GoalModel goal)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<GoalModel>.FailResponse(ModelState));
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<GoalModel>.FailResponse("Utilizador não autenticado."));

            goal.UserId = userId;
            await _service.CreateGoalAsync(goal);
            return Ok(APIResponse<GoalModel>.SuccessResponse(goal, "Meta criada com sucesso."));
        }

        /// <summary>
        /// Atualiza uma meta existente.
        /// </summary>
        [HttpPut("{stampEntity}")]
        public async Task<IActionResult> UpdateGoal(string stampEntity, [FromBody] GoalModel goal)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<GoalModel>.FailResponse(ModelState));

            goal.StampEntity = stampEntity;
            await _service.UpdateGoalAsync(goal);
            return Ok(APIResponse<GoalModel>.SuccessResponse(goal, "Meta atualizada com sucesso."));
        }

        /// <summary>
        /// Remove uma meta específica.
        /// </summary>
        [HttpDelete("{stampEntity}")]
        public async Task<IActionResult> DeleteGoal(string stampEntity)
        {
            await _service.DeleteGoalAsync(stampEntity);
            return Ok(APIResponse<object>.SuccessResponse(null, "Meta removida com sucesso."));
        }
    }
}
