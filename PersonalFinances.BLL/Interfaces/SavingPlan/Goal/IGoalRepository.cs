using PersonalFinances.BLL.Entities.Models.SavingPlan;

namespace PersonalFinances.BLL.Interfaces.SavingPlan.Goal
{
    public interface IGoalRepository
    {
        Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId);
        Task<GoalModel> GetGoalByIdAsync(string goalId);
        Task CreateGoalAsync(GoalModel goal);
        Task UpdateGoalAsync(GoalModel goal);
        Task DeleteGoalAsync(string goalId);
        Task<IEnumerable<GoalProgressModel>> GetGoalProgressAsync(string goalId);
        Task AddGoalProgressAsync(string goalId, decimal valorAtual);
    }
}
