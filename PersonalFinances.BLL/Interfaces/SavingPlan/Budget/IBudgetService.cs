using PersonalFinances.BLL.Entities.Models.SavingPlan;

namespace PersonalFinances.BLL.Interfaces.SavingPlan.Budget
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId);
        Task<BudgetModel> GetBudgetByIdAsync(string budgetId);
        Task CreateBudgetAsync(BudgetModel budget);
        Task UpdateBudgetAsync(BudgetModel budget);
        Task DeleteBudgetAsync(string budgetId);
        Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId);
        Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto);
    }
}
