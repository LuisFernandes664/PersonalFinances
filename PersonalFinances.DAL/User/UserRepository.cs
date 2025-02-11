using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Interfaces.User;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _dbContext;

        public UserRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModel> GetUserByStampEntity(string stampEntity)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StampEntity", stampEntity)
            };

            var row = await SQLHelper.ExecuteScalarAsync("SELECT * FROM Users WHERE stamp_entity = @StampEntity", parameters);

            if (row != null)
            {
                return new UserModel(row);
            }

            return null;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@email", email)
            };

            var row = await SQLHelper.ExecuteScalarAsync("SELECT * FROM Users WHERE email = @email", parameters);

            if (row != null)
            {
                return new UserModel(row);
            }

            return null;
        }

        public async Task SaveUser(UserModel user)
        {
            var parameters = new List<SqlParameter>
            {
                
                new SqlParameter("@StampEntity", Guid.NewGuid()),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@CreatedAt", user.CreatedAt),
                new SqlParameter("@UpdatedAt", user.UpdatedAt),
            };

            await SQLHelper.ExecuteNonQueryAsync(
                "INSERT INTO Users(stamp_entity, user_name, password, email, created_at, updated_at) VALUES(@StampEntity, @Username, @Password, @Email, @CreatedAt, @UpdatedAt)",
                parameters
            );
        }

        public async Task UpdateUser(UserModel user)
        {
            user.UpdateDate();
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@StampEntity", user.StampEntity),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PhoneNumber", user.PhoneNumber),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@UpdatedAt", user.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(
                "UPDATE Users SET user_name = @Username, email = @Email, phone_number = @PhoneNumber, password = @Password, updated_at = @UpdatedAt WHERE stamp_entity = @StampEntity",
                parameters
            );
        }


    }


}
