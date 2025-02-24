using PersonalFinances.BLL.Entities.Models.SavingPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.SavingPlan.Budget
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId);
        Task CreateBudgetAsync(BudgetModel budget);
        Task UpdateBudgetAsync(BudgetModel budget);
        Task DeleteBudgetAsync(string stampEntity);
    }
}
