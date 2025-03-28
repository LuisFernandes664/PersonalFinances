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

        public async Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId) => await _repository.GetGoalsByUserAsync(userId);

        //public async Task<IEnumerable<SelectListItem>> GetCategoriesAsync() => await _repository.GetCategoriesAsync();
        
        public async Task<GoalModel> GetGoalByIdAsync(string goalId) => await _repository.GetGoalByIdAsync(goalId);

        public async Task CreateGoalAsync(GoalModel goal) => await _repository.CreateGoalAsync(goal);

        public async Task UpdateGoalAsync(GoalModel goal) => await _repository.UpdateGoalAsync(goal);

        public async Task DeleteGoalAsync(string goalId) => await _repository.DeleteGoalAsync(goalId);

        public async Task<IEnumerable<GoalProgressModel>> GetGoalProgressAsync(string goalId) => await _repository.GetGoalProgressAsync(goalId);

        public async Task AddGoalProgressAsync(string goalId, decimal valorAtual) => await _repository.AddGoalProgressAsync(goalId, valorAtual);

        public async Task UpdateGoalAccumulatedAmount(string goalId) => await _repository.UpdateGoalAccumulatedAmount(goalId);

        public Task<decimal> GetAccumulatedAmountByGoal(string goalId) => _repository.GetAccumulatedAmountByGoal(goalId);

        public Task<decimal> GetGoalProgressPercentage(string goalId) => _repository.GetGoalProgressPercentage(goalId);
    }
}
