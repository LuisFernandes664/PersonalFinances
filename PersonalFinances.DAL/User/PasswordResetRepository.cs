using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Interfaces.Repository;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.User
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly DatabaseContext _dbContext;

        public PasswordResetRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        // 4493
        public async Task SaveAsync(PasswordResetRequestModel request)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserStamp", request.UserStamp),
                new SqlParameter("@StampEntity", request.StampEntity),
                new SqlParameter("@Token", request.Token),
                new SqlParameter("@CreatedAt", request.CreatedAt),
                new SqlParameter("@ExpiresAt", request.ExpiresAt),
                new SqlParameter("@Used", request.Used)
            };

            await SQLHelper.ExecuteNonQueryAsync(
                @"INSERT INTO PasswordResetRequests (stamp_entity, user_stamp, token, created_at, expires_at, used) VALUES (@StampEntity, @UserStamp, @Token, @CreatedAt, @ExpiresAt, @Used)",
                parameters
            );
        }


        public async Task<PasswordResetRequestModel> GetByToken(string token)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Token", token)
            };

            var row = await SQLHelper.ExecuteScalarAsync("SELECT * FROM PasswordResetRequests WHERE token = @Token", parameters);

            if (row != null)
            {
                return new PasswordResetRequestModel(row);
            }

            return null;
        }


        public async Task UpdateAsync(PasswordResetRequestModel request)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Used", request.Used),
                new SqlParameter("@StampEntity", request.StampEntity)
            };

            await SQLHelper.ExecuteNonQueryAsync(
                "UPDATE PasswordResetRequests SET used = @Used WHERE stamp_entity = @StampEntity",
                parameters
            );
        }

        public async Task DeleteAsync(string userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                throw new ArgumentException("O identificador do utilizador não pode ser nulo ou vazio.", nameof(userID));
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserID", userID)
            };

            const string query = "DELETE FROM PasswordResetRequests WHERE stamp_entity = @UserID";

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

    }

}
