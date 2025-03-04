using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.SavingPlan.Budget
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repository;

        public BudgetService(IBudgetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId) => await _repository.GetBudgetsByUserAsync(userId);

        public async Task<BudgetModel> GetBudgetByIdAsync(string budgetId) => await _repository.GetBudgetByIdAsync(budgetId);

        public async Task<decimal> GetSpentAmountByBudget(string budgetId) => await _repository.GetSpentAmountByBudget(budgetId);

        public async Task CreateBudgetAsync(BudgetModel budget) => await _repository.CreateBudgetAsync(budget);

        public async Task UpdateBudgetAsync(BudgetModel budget) => await _repository.UpdateBudgetAsync(budget);

        public async Task DeleteBudgetAsync(string budgetId) => await _repository.DeleteBudgetAsync(budgetId);

        public async Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId) => await _repository.GetBudgetHistoryAsync(budgetId);

        public async Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto) => await _repository.AddBudgetHistoryAsync(budgetId, transactionId, valorGasto);

        public async Task UpdateBudgetSpentAmount(string budgetId) => await _repository.UpdateBudgetSpentAmount(budgetId);

    }
}
