using PersonalFinances.BLL.Entities.Models.SavingPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.SavingPlan.Goal
{
    public interface IGoalRepository
    {
        Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId);
        Task CreateGoalAsync(GoalModel goal);
        Task UpdateGoalAsync(GoalModel goal);
        Task DeleteGoalAsync(string stampEntity);
    }
}
