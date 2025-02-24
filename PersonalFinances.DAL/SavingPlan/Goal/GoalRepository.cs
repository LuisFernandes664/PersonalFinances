using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.SavingPlan.Goal
{
    public class GoalRepository : IGoalRepository
    {
        private readonly string _connectionString;
        public GoalRepository()
        {
            _connectionString = ConfigManager.GetConnectionString();
        }

        public async Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId)
        {
            var goals = new List<GoalModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Goals WHERE UserId = @userId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        goals.Add(new GoalModel
                        {
                            StampEntity = reader["StampEntity"].ToString(),
                            UserId = reader["UserId"].ToString(),
                            Descricao = reader["Descricao"].ToString(),
                            ValorAlvo = Convert.ToDecimal(reader["ValorAlvo"]),
                            ValorAtual = Convert.ToDecimal(reader["ValorAtual"]),
                            DataLimite = Convert.ToDateTime(reader["DataLimite"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return goals;
        }

        public async Task CreateGoalAsync(GoalModel goal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO Goals 
                          (StampEntity, UserId, Descricao, ValorAlvo, ValorAtual, DataLimite, CreatedAt)
                          VALUES (@stampEntity, @userId, @descricao, @valorAlvo, @valorAtual, @dataLimite, GETDATE());";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@stampEntity", goal.StampEntity);
                command.Parameters.AddWithValue("@userId", goal.UserId);
                command.Parameters.AddWithValue("@descricao", goal.Descricao);
                command.Parameters.AddWithValue("@valorAlvo", goal.ValorAlvo);
                command.Parameters.AddWithValue("@valorAtual", goal.ValorAtual);
                command.Parameters.AddWithValue("@dataLimite", goal.DataLimite);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateGoalAsync(GoalModel goal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"UPDATE Goals SET 
                          Descricao = @descricao,
                          ValorAlvo = @valorAlvo,
                          ValorAtual = @valorAtual,
                          DataLimite = @dataLimite
                          WHERE StampEntity = @stampEntity";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@descricao", goal.Descricao);
                command.Parameters.AddWithValue("@valorAlvo", goal.ValorAlvo);
                command.Parameters.AddWithValue("@valorAtual", goal.ValorAtual);
                command.Parameters.AddWithValue("@dataLimite", goal.DataLimite);
                command.Parameters.AddWithValue("@stampEntity", goal.StampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteGoalAsync(string stampEntity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Goals WHERE StampEntity = @stampEntity";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@stampEntity", stampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
