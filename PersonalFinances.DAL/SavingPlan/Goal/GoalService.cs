using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.SavingPlan.Goal
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

        public async Task CreateGoalAsync(GoalModel goal)
        {
            goal.StampEntity = Guid.NewGuid().ToString();
            await _repository.CreateGoalAsync(goal);
        }

        public async Task UpdateGoalAsync(GoalModel goal)
        {
            await _repository.UpdateGoalAsync(goal);
        }

        public async Task DeleteGoalAsync(string stampEntity)
        {
            await _repository.DeleteGoalAsync(stampEntity);
        }
    }

}
