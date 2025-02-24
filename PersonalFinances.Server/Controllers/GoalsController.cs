using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;

namespace PersonalFinances.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _service;
        public GoalsController(IGoalService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetGoalsByUser(string userId)
        {
            var goals = await _service.GetGoalsByUserAsync(userId);
            return Ok(APIResponse<IEnumerable<GoalModel>>.SuccessResponse(goals, "Metas obtidas com sucesso."));
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] GoalModel goal)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<GoalModel>.FailResponse(ModelState));

            await _service.CreateGoalAsync(goal);
            return Ok(APIResponse<GoalModel>.SuccessResponse(goal, "Meta criada com sucesso."));
        }

        [HttpPut("{stampEntity}")]
        public async Task<IActionResult> UpdateGoal(string stampEntity, [FromBody] GoalModel goal)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<GoalModel>.FailResponse(ModelState));

            goal.StampEntity = stampEntity;
            await _service.UpdateGoalAsync(goal);
            return Ok(APIResponse<GoalModel>.SuccessResponse(goal, "Meta atualizada com sucesso."));
        }

        [HttpDelete("{stampEntity}")]
        public async Task<IActionResult> DeleteGoal(string stampEntity)
        {
            await _service.DeleteGoalAsync(stampEntity);
            return Ok(APIResponse<object>.SuccessResponse(null, "Meta removida com sucesso."));
        }

    }

}
