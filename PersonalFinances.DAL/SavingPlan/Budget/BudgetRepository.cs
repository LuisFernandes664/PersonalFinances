using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.SavingPlan.Budget
{
    public class BudgetRepository : IBudgetRepository
    {
        public async Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId)
        {
            var budgets = new List<BudgetModel>();
            var parameters = new List<SqlParameter> { new("@userId", userId) };
            var query = "SELECT * FROM Budgets WHERE user_id = @userId";

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in result.Rows)
                budgets.Add(new BudgetModel(row));

            return budgets;
        }

        public async Task<BudgetModel> GetBudgetByIdAsync(string budgetId)
        {
            BudgetModel budget = null;
            var parameters = new List<SqlParameter> { new("@budgetId", budgetId) };
            var query = "SELECT * FROM Budgets WHERE stamp_entity = @budgetId";

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
                budget = new BudgetModel(result.Rows[0]);

            return budget;
        }

        public async Task<decimal> GetSpentAmountByBudget(string budgetId)
        {
            var query = @"SELECT COALESCE(SUM(amount), 0) 
                  FROM Transactions 
                  WHERE reference_id = @budgetId 
                  AND reference_type = 'Budget'";

            var parameters = new List<SqlParameter> { new("@budgetId", budgetId) };
            var result = await SQLHelper.ExecuteScalarAsync(query, parameters);

            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task CreateBudgetAsync(BudgetModel budget)
        {
            var getCategoryQuery = "SELECT stamp_entity FROM Categories WHERE name = @name AND type = 'budget'";
            var categoryParams = new List<SqlParameter> { new("@name", budget.CategoryId) };
            var result = await SQLHelper.ExecuteScalarAsync(getCategoryQuery, categoryParams);

            string categoryId = result?.ToString();
            if (categoryId == null)
                throw new Exception("Categoria inválida.");

            var query = @"INSERT INTO Budgets 
                          (stamp_entity, user_id, category_id, valor_orcado, data_inicio, data_fim, created_at)
                          VALUES (@stampEntity, @userId, @categoryId, @valorOrcado, @dataInicio, @dataFim, GETDATE());";

            var parameters = new List<SqlParameter>
            {
                new("@stampEntity", budget.StampEntity),
                new("@userId", budget.UserId),
                new("@categoryId", categoryId),
                new("@valorOrcado", budget.ValorOrcado),
                new("@dataInicio", budget.DataInicio),
                new("@dataFim", budget.DataFim)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task UpdateBudgetAsync(BudgetModel budget)
        {
            var query = @"UPDATE Budgets SET 
                          category_id = @categoryId,
                          valor_orcado = @valorOrcado,
                          data_inicio = @dataInicio,
                          data_fim = @dataFim
                          WHERE stamp_entity = @stampEntity";

            var parameters = new List<SqlParameter>
            {
                new("@categoryId", budget.CategoryId),
                new("@valorOrcado", budget.ValorOrcado),
                new("@dataInicio", budget.DataInicio),
                new("@dataFim", budget.DataFim),
                new("@stampEntity", budget.StampEntity)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteBudgetAsync(string budgetId)
        {
            var query = "DELETE FROM Budgets WHERE stamp_entity = @budgetId";
            var parameters = new List<SqlParameter> { new("@budgetId", budgetId) };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId)
        {
            var history = new List<BudgetHistoryModel>();
            var parameters = new List<SqlParameter> { new("@budgetId", budgetId) };
            var query = "SELECT * FROM BudgetHistory WHERE budget_id = @budgetId";

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in result.Rows)
                history.Add(new BudgetHistoryModel(row));

            return history;
        }

        public async Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto)
        {
            var query = @"INSERT INTO BudgetHistory 
                          (stamp_entity, budget_id, transaction_id, valor_gasto, data_registro)
                          VALUES (NEWID(), @budgetId, @transactionId, @valorGasto, GETDATE());";

            var parameters = new List<SqlParameter>
            {
                new("@budgetId", budgetId),
                new("@transactionId", transactionId),
                new("@valorGasto", valorGasto)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task UpdateBudgetSpentAmount(string budgetId)
        {
            var query = @"UPDATE Budgets SET valor_gasto = (SELECT COALESCE(SUM(amount), 0) FROM Transactions WHERE reference_id = @budgetId AND reference_type = 'Budget') WHERE stamp_entity = @budgetId";

            var parameters = new List<SqlParameter>
            {
                new("@budgetId", budgetId)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

    }
}
