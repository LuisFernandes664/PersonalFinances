using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        public async Task<IEnumerable<TransactionModel>> GetAllAsync(string userStamp)
        {
            var query = "SELECT * FROM Transactions WHERE user_stamp = @userStamp";
            var parameters = new List<SqlParameter> { new("@userStamp", userStamp) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            return result.Rows.Cast<DataRow>().Select(MapTransaction).ToList();
        }

        public async Task<TransactionModel> GetByStampEntityAsync(string stampEntity)
        {
            var query = "SELECT * FROM Transactions WHERE stamp_entity = @stamp";
            var parameters = new List<SqlParameter> { new("@stamp", stampEntity) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            return result.Rows.Count > 0 ? MapTransaction(result.Rows[0]) : null;
        }

        public async Task AddAsync(TransactionModel transaction)
        {
            var query = @"INSERT INTO Transactions 
                        (stamp_entity, user_stamp, description, amount, date, category, paymentMethod, recipient, status, created_at, updated_at)
                        VALUES (@stampEntity, @userStamp, @description, @amount, @date, @category, @paymentMethod, @recipient, @status, GETDATE(), GETDATE());";

            var parameters = GetTransactionParameters(transaction);
            parameters.Add(new SqlParameter("@userStamp", transaction.UserStamp));

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }


        public async Task UpdateAsync(TransactionModel transaction)
        {
            var query = @"UPDATE Transactions SET 
                          description = @description,
                          amount = @amount,
                          date = @date,
                          category = @category,
                          paymentMethod = @paymentMethod,
                          recipient = @recipient,
                          status = @status,
                          updated_at = GETDATE()
                          WHERE stamp_entity = @stamp";

            var parameters = GetTransactionParameters(transaction);
            parameters.Add(new SqlParameter("@stamp", transaction.StampEntity));

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteAsync(string stampEntity)
        {
            var query = "DELETE FROM Transactions WHERE stamp_entity = @stamp";
            var parameters = new List<SqlParameter> { new("@stamp", stampEntity) };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        /// <summary>
        /// Mapeia um DataRow para um TransactionModel.
        /// </summary>
        private TransactionModel MapTransaction(DataRow row)
        {
            return new TransactionModel
            {
                StampEntity = row["stamp_entity"].ToString(),
                Description = row["description"].ToString(),
                Amount = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0,
                Date = row["date"] != DBNull.Value ? Convert.ToDateTime(row["date"]) : default,
                Category = row["category"].ToString(),
                PaymentMethod = row["paymentMethod"].ToString(),
                Recipient = row["recipient"].ToString(),
                Status = row["status"].ToString(),
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : default,
                UpdatedAt = row["updated_at"] != DBNull.Value ? Convert.ToDateTime(row["updated_at"]) : default
            };
        }

        /// <summary>
        /// Gera a lista de parâmetros SQL para evitar código repetitivo.
        /// </summary>
        private List<SqlParameter> GetTransactionParameters(TransactionModel transaction)
        {
            return new List<SqlParameter>
            {
                new("@stampEntity", transaction.StampEntity),
                new("@description", transaction.Description),
                new("@amount", transaction.Amount),
                new("@date", transaction.Date),
                new("@category", transaction.Category),
                new("@paymentMethod", transaction.PaymentMethod),
                new("@recipient", transaction.Recipient),
                new("@status", transaction.Status)
            };
        }
    }
}
