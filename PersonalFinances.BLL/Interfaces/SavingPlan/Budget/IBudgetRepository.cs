using PersonalFinances.BLL.Entities.Models.Analytics;
using PersonalFinances.BLL.Entities.Models.SavingPlan;

namespace PersonalFinances.BLL.Interfaces.SavingPlan.Budget
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId);
        Task<BudgetModel> GetBudgetByIdAsync(string budgetId);
        Task<decimal> GetSpentAmountByBudget(string budgetId);
        Task CreateBudgetAsync(BudgetModel budget);
        Task UpdateBudgetAsync(BudgetModel budget);
        Task DeleteBudgetAsync(string budgetId);
        Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId);
        Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto);
        Task UpdateBudgetSpentAmount(string budgetId);
        Task<List<HistoricalSpendingModel>> GetHistoricalSpendingByCategory(string categoryId, int months);
        Task<string> GetCategoryNameById(string categoryId);
    }
}
