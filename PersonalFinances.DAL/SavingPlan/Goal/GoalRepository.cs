using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.SavingPlan.Goal
{
    public class GoalRepository : IGoalRepository
    {
        public async Task<IEnumerable<GoalModel>> GetGoalsByUserAsync(string userId)
        {
            var goals = new List<GoalModel>();
            var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };
            var query = 
                "SELECT g.*, COALESCE((SELECT TOP 1 gp.valor_atual FROM GoalProgress gp WHERE gp.goal_id = g.stamp_entity ORDER BY gp.data_registro DESC), 0) AS valor_atual " +
                "FROM Goals g " +
                "WHERE g.user_id = @userId";
            
            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in result.Rows)
                goals.Add(new GoalModel(row));

            return goals;
        }

        public async Task<GoalModel> GetGoalByIdAsync(string goalId)
        {
            GoalModel goal = null;
            var parameters = new List<SqlParameter> { new SqlParameter("@goalId", goalId) };
            var query = 
                "SELECT g.*, COALESCE((SELECT TOP 1 gp.valor_atual FROM GoalProgress gp WHERE gp.goal_id = g.stamp_entity ORDER BY gp.data_registro DESC), 0) AS valor_atual " +
                "FROM Goals g " +
                "WHERE g.stamp_entity = @goalId";

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
                goal = new GoalModel(result.Rows[0]);

            return goal;
        }

        //public async Task<IEnumerable<SelectListItem>> GetCategoriesAsync()
        //{
        //    var categories = new List<SelectListItem>();
        //    var query = "SELECT stamp_entity, name FROM Categories WHERE type = 'goal'";

        //    var result = await SQLHelper.ExecuteQueryAsync(query);
        //    foreach (DataRow row in result.Rows)
        //    {
        //        categories.Add(new SelectListItem
        //        {
        //            Text = row["name"].ToString(),
        //            Value = row["stamp_entity"].ToString()
        //        });
        //    }

        //    return categories;
        //}

        public async Task<decimal> GetAccumulatedAmountByGoal(string goalId)
        {
            var query = @"SELECT COALESCE(SUM(amount), 0) 
                  FROM Transactions 
                  WHERE reference_id = @goalId 
                  AND reference_type = 'Goal'";

            var parameters = new List<SqlParameter> { new("@goalId", goalId) };
            var result = await SQLHelper.ExecuteScalarAsync(query, parameters);

            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task<decimal> GetGoalProgressPercentage(string goalId)
        {
            var query = @"SELECT (COALESCE(SUM(amount), 0) / g.valor_alvo) * 100
                  FROM Transactions t
                  JOIN Goals g ON t.reference_id = g.stamp_entity
                  WHERE t.reference_id = @goalId 
                  AND t.reference_type = 'Goal'";

            var parameters = new List<SqlParameter> { new("@goalId", goalId) };
            var result = await SQLHelper.ExecuteScalarAsync(query, parameters);

            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task CreateGoalAsync(GoalModel goal)
        {
            var getCategoryQuery = "SELECT stamp_entity FROM Categories WHERE stamp_entity = @categoryID AND type = 'goal'";
            var categoryParams = new List<SqlParameter> { new SqlParameter("@categoryID", goal.CategoryId) };
            var result = await SQLHelper.ExecuteScalarAsync(getCategoryQuery, categoryParams);

            string categoryId = result?.ToString();
            if (categoryId == null)
                throw new Exception("Categoria inválida.");

            var query = 
                "INSERT INTO Goals (stamp_entity, user_id, category_id, descricao, valor_alvo, data_limite, created_at)" +
                "VALUES (@stampEntity, @userId, @categoryId, @descricao, @valorAlvo, @dataLimite, GETDATE());";
            
            var parameters = new List<SqlParameter>
            {
                new("@stampEntity", goal.StampEntity),
                new("@userId", goal.UserId),
                new("@categoryId", goal.CategoryId),
                new("@descricao", goal.Descricao),
                new("@valorAlvo", goal.ValorAlvo),
                new("@dataLimite", goal.DataLimite)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task UpdateGoalAsync(GoalModel goal)
        {
            var query = @"UPDATE Goals SET category_id = @categoryId, descricao = @descricao, valor_alvo = @valorAlvo, data_limite = @dataLimite WHERE stamp_entity = @goalId";

            var parameters = new List<SqlParameter>
            {
                new("@categoryId", goal.CategoryId),
                new("@descricao", goal.Descricao),
                new("@valorAlvo", goal.ValorAlvo),
                new("@dataLimite", goal.DataLimite),
                new("@goalId", goal.StampEntity)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteGoalAsync(string goalId)
        {
            var query = "DELETE FROM Goals WHERE stamp_entity = @goalId";
            var parameters = new List<SqlParameter> { new("@goalId", goalId) };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<IEnumerable<GoalProgressModel>> GetGoalProgressAsync(string goalId)
        {
            var progressList = new List<GoalProgressModel>();
            var parameters = new List<SqlParameter> { new("@goalId", goalId) };
            var query = "SELECT * FROM GoalProgress WHERE goal_id = @goalId ORDER BY data_registro DESC";

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            foreach (DataRow row in result.Rows)
                progressList.Add(new GoalProgressModel(row));

            return progressList;
        }

        public async Task AddGoalProgressAsync(string goalId, decimal valorAtual)
        {
            var query = @"INSERT INTO GoalProgress (stamp_entity, goal_id, valor_atual, data_registro) VALUES (NEWID(), @goalId, @valorAtual, GETDATE());";

            var parameters = new List<SqlParameter>
            {
                new("@goalId", goalId),
                new("@valorAtual", valorAtual)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task UpdateGoalAccumulatedAmount(string goalId)
        {
            var query = @"UPDATE Goals 
                        SET valor_atual = (SELECT COALESCE(SUM(amount), 0) FROM Transactions WHERE reference_id = @goalId AND reference_type = 'Goal')
                        WHERE stamp_entity = @goalId";

            var parameters = new List<SqlParameter>
            {
                new("@goalId", goalId)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

    }
}
