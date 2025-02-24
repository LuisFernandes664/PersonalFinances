using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.SavingPlan.Budget
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repository;
        public BudgetService(IBudgetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId)
        {
            return await _repository.GetBudgetsByUserAsync(userId);
        }

        public async Task CreateBudgetAsync(BudgetModel budget)
        {
            budget.StampEntity = Guid.NewGuid().ToString();
            await _repository.CreateBudgetAsync(budget);
        }

        public async Task UpdateBudgetAsync(BudgetModel budget)
        {
            await _repository.UpdateBudgetAsync(budget);
        }

        public async Task DeleteBudgetAsync(string stampEntity)
        {
            await _repository.DeleteBudgetAsync(stampEntity);
        }
    }

}
