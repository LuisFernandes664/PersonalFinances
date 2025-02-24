using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
                var query = "SELECT * FROM Budgets WHERE UserId = @userId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        budgets.Add(new BudgetModel
                        {
                            StampEntity = reader["StampEntity"].ToString(),
                            UserId = reader["UserId"].ToString(),
                            Categoria = reader["Categoria"].ToString(),
                            ValorOrcado = Convert.ToDecimal(reader["ValorOrcado"]),
                            DataInicio = Convert.ToDateTime(reader["DataInicio"]),
                            DataFim = Convert.ToDateTime(reader["DataFim"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return budgets;
        }

        public async Task CreateBudgetAsync(BudgetModel budget)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO Budgets 
                          (StampEntity, UserId, Categoria, ValorOrcado, DataInicio, DataFim, CreatedAt)
                          VALUES (@stampEntity, @userId, @categoria, @valorOrcado, @dataInicio, @dataFim, GETDATE());";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@stampEntity", budget.StampEntity);
                command.Parameters.AddWithValue("@userId", budget.UserId);
                command.Parameters.AddWithValue("@categoria", budget.Categoria);
                command.Parameters.AddWithValue("@valorOrcado", budget.ValorOrcado);
                command.Parameters.AddWithValue("@dataInicio", budget.DataInicio);
                command.Parameters.AddWithValue("@dataFim", budget.DataFim);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateBudgetAsync(BudgetModel budget)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"UPDATE Budgets SET 
                          Categoria = @categoria,
                          ValorOrcado = @valorOrcado,
                          DataInicio = @dataInicio,
                          DataFim = @dataFim
                          WHERE StampEntity = @stampEntity";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@categoria", budget.Categoria);
                command.Parameters.AddWithValue("@valorOrcado", budget.ValorOrcado);
                command.Parameters.AddWithValue("@dataInicio", budget.DataInicio);
                command.Parameters.AddWithValue("@dataFim", budget.DataFim);
                command.Parameters.AddWithValue("@stampEntity", budget.StampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteBudgetAsync(string stampEntity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Budgets WHERE StampEntity = @stampEntity";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@stampEntity", stampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
