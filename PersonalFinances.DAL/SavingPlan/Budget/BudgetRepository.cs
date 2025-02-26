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
        private readonly string _connectionString;
        public BudgetRepository()
        {
            _connectionString = ConfigManager.GetConnectionString();
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId)
        {
            var budgets = new List<BudgetModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT * FROM Budgets WHERE user_id = @userId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        budgets.Add(new BudgetModel(row));
                    }
                }
            }
            return budgets;
        }

        public async Task<BudgetModel> GetBudgetByIdAsync(string budgetId)
        {
            BudgetModel budget = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Budgets WHERE stamp_entity = @budgetId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@budgetId", budgetId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    if (dataTable.Rows.Count > 0)
                    {
                        budget = new BudgetModel(dataTable.Rows[0]);
                    }
                }
            }
            return budget;
        }

        public async Task CreateBudgetAsync(BudgetModel budget)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Buscar o ID da categoria pelo nome
                var getCategoryQuery = "SELECT stamp_entity FROM Categories WHERE name = @name AND type = 'budget'";
                string categoryId = null;

                using (var command = new SqlCommand(getCategoryQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", budget.CategoryId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoryId = reader["stamp_entity"].ToString();
                        }
                    }
                }

                if (categoryId == null)
                    throw new Exception("Categoria inválida.");

                // Criar o orçamento com a categoria correspondente
                var query = @"INSERT INTO Budgets 
                      (stamp_entity, user_id, category_id, valor_orcado, data_inicio, data_fim, created_at)
                      VALUES (@stampEntity, @userId, @categoryId, @valorOrcado, @dataInicio, @dataFim, GETDATE());";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@stampEntity", budget.StampEntity);
                    command.Parameters.AddWithValue("@userId", budget.UserId);
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    command.Parameters.AddWithValue("@valorOrcado", budget.ValorOrcado);
                    command.Parameters.AddWithValue("@dataInicio", budget.DataInicio);
                    command.Parameters.AddWithValue("@dataFim", budget.DataFim);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task UpdateBudgetAsync(BudgetModel budget)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"UPDATE Budgets SET 
                              category_id = @categoryId,
                              valor_orcado = @valorOrcado,
                              data_inicio = @dataInicio,
                              data_fim = @dataFim
                              WHERE stamp_entity = @stampEntity";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@categoryId", budget.CategoryId);
                command.Parameters.AddWithValue("@valorOrcado", budget.ValorOrcado);
                command.Parameters.AddWithValue("@dataInicio", budget.DataInicio);
                command.Parameters.AddWithValue("@dataFim", budget.DataFim);
                command.Parameters.AddWithValue("@stampEntity", budget.StampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteBudgetAsync(string budgetId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Budgets WHERE stamp_entity = @budgetId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@budgetId", budgetId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId)
        {
            var history = new List<BudgetHistoryModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT * FROM BudgetHistory WHERE budget_id = @budgetId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@budgetId", budgetId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        history.Add(new BudgetHistoryModel(row));
                    }
                }
            }
            return history;
        }


        public async Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO BudgetHistory 
                              (stamp_entity, budget_id, transaction_id, valor_gasto, data_registro)
                              VALUES (NEWID(), @budgetId, @transactionId, @valorGasto, GETDATE());";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@budgetId", budgetId);
                command.Parameters.AddWithValue("@transactionId", transactionId);
                command.Parameters.AddWithValue("@valorGasto", valorGasto);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
