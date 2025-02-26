using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                var query = @"SELECT 
                                g.*, 
                                COALESCE(
                                    (SELECT TOP 1 gp.valor_atual 
                                     FROM GoalProgress gp 
                                     WHERE gp.goal_id = g.stamp_entity 
                                     ORDER BY gp.data_registro DESC), 0) AS valor_atual
                              FROM Goals g
                              WHERE g.user_id = @userId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var goal = new GoalModel
                        {
                            StampEntity = reader["stamp_entity"].ToString(),
                            UserId = reader["user_id"].ToString(),
                            CategoryId = reader["category_id"].ToString(),
                            Descricao = reader["descricao"].ToString(),
                            ValorAlvo = Convert.ToDecimal(reader["valor_alvo"]),
                            DataLimite = Convert.ToDateTime(reader["data_limite"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            // ValorAtual é obtido a partir do GoalProgress
                            ValorAtual = Convert.ToDecimal(reader["valor_atual"])
                        };
                        goals.Add(goal);
                    }
                }
            }
            return goals;
        }

        public async Task<GoalModel> GetGoalByIdAsync(string goalId)
        {
            GoalModel goal = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT 
                                g.*, 
                                COALESCE(
                                    (SELECT TOP 1 gp.valor_atual 
                                     FROM GoalProgress gp 
                                     WHERE gp.goal_id = g.stamp_entity 
                                     ORDER BY gp.data_registro DESC), 0) AS valor_atual
                              FROM Goals g
                              WHERE g.stamp_entity = @goalId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@goalId", goalId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        goal = new GoalModel
                        {
                            StampEntity = reader["stamp_entity"].ToString(),
                            UserId = reader["user_id"].ToString(),
                            CategoryId = reader["category_id"].ToString(),
                            Descricao = reader["descricao"].ToString(),
                            ValorAlvo = Convert.ToDecimal(reader["valor_alvo"]),
                            DataLimite = Convert.ToDateTime(reader["data_limite"]),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            ValorAtual = Convert.ToDecimal(reader["valor_atual"])
                        };
                    }
                }
            }
            return goal;
        }

        public async Task CreateGoalAsync(GoalModel goal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Buscar ID da categoria correspondente
                var getCategoryQuery = "SELECT stamp_entity FROM Categories WHERE name = @name AND type = 'goal'";
                string categoryId = null;

                using (var command = new SqlCommand(getCategoryQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", goal.Descricao);
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

                // Criar a meta com a categoria correta
                var query = @"INSERT INTO Goals 
                      (stamp_entity, user_id, category_id, descricao, valor_alvo, data_limite, created_at)
                      VALUES (@stampEntity, @userId, @categoryId, @descricao, @valorAlvo, @dataLimite, GETDATE());";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@stampEntity", goal.StampEntity);
                    command.Parameters.AddWithValue("@userId", goal.UserId);
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                    command.Parameters.AddWithValue("@descricao", goal.Descricao);
                    command.Parameters.AddWithValue("@valorAlvo", goal.ValorAlvo);
                    command.Parameters.AddWithValue("@dataLimite", goal.DataLimite);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task UpdateGoalAsync(GoalModel goal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"UPDATE Goals SET 
                              category_id = @categoryId,
                              descricao = @descricao,
                              valor_alvo = @valorAlvo,
                              data_limite = @dataLimite
                              WHERE stamp_entity = @goalId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@categoryId", goal.CategoryId);
                command.Parameters.AddWithValue("@descricao", goal.Descricao);
                command.Parameters.AddWithValue("@valorAlvo", goal.ValorAlvo);
                command.Parameters.AddWithValue("@dataLimite", goal.DataLimite);
                command.Parameters.AddWithValue("@goalId", goal.StampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteGoalAsync(string goalId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Goals WHERE stamp_entity = @goalId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@goalId", goalId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<GoalProgressModel>> GetGoalProgressAsync(string goalId)
        {
            var progressList = new List<GoalProgressModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"SELECT * FROM GoalProgress WHERE goal_id = @goalId ORDER BY data_registro DESC";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@goalId", goalId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        progressList.Add(new GoalProgressModel
                        {
                            StampEntity = reader["stamp_entity"].ToString(),
                            GoalId = reader["goal_id"].ToString(),
                            ValorAtual = Convert.ToDecimal(reader["valor_atual"]),
                            DataRegistro = Convert.ToDateTime(reader["data_registro"])
                        });
                    }
                }
            }
            return progressList;
        }

        public async Task AddGoalProgressAsync(string goalId, decimal valorAtual)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO GoalProgress 
                              (stamp_entity, goal_id, valor_atual, data_registro)
                              VALUES (NEWID(), @goalId, @valorAtual, GETDATE());";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@goalId", goalId);
                command.Parameters.AddWithValue("@valorAtual", valorAtual);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
