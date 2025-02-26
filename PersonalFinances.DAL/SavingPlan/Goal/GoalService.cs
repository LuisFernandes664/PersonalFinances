using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.SavingPlan.Goal
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _repository;
        public GoalService(IGoalRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId)
        {
            return await _repository.GetGoalsByUserAsync(userId);
        }

        public async Task<GoalModel> GetGoalByIdAsync(string goalId)
        {
            return await _repository.GetGoalByIdAsync(goalId);
        }

        public async Task CreateGoalAsync(GoalModel goal)
        {
            await _repository.CreateGoalAsync(goal);
        }

        public async Task UpdateGoalAsync(GoalModel goal)
        {
            await _repository.UpdateGoalAsync(goal);
        }

        public async Task DeleteGoalAsync(string goalId)
        {
            await _repository.DeleteGoalAsync(goalId);
        }

        public async Task<IEnumerable<GoalProgressModel>> GetGoalProgressAsync(string goalId)
        {
            return await _repository.GetGoalProgressAsync(goalId);
        }

        public async Task AddGoalProgressAsync(string goalId, decimal valorAtual)
        {
            await _repository.AddGoalProgressAsync(goalId, valorAtual);
        }
    }
}
