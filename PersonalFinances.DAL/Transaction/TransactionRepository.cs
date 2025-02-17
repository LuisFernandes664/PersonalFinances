using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Transaction
{

    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository()
        {
            _connectionString = ConfigManager.GetConnectionString();
        }

        public async Task<IEnumerable<TransactionModel>> GetAllAsync()
        {
            var transactions = new List<TransactionModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Transactions", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        transactions.Add(new TransactionModel
                        {
                            StampEntity = reader["stamp_entity"].ToString(),
                            Description = reader["description"].ToString(),
                            Amount = Convert.ToDecimal(reader["amount"]),
                            Date = Convert.ToDateTime(reader["date"]),
                            Category = reader["category"].ToString(),
                            PaymentMethod = reader["paymentMethod"].ToString(),
                            Recipient = reader["recipient"].ToString(),
                            Status = reader["status"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"])
                        });
                    }
                }
            }
            return transactions;
        }

        public async Task<TransactionModel> GetByStampEntityAsync(string stampEntity)
        {
            TransactionModel transaction = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Transactions WHERE stamp_entity = @stamp", connection);
                command.Parameters.AddWithValue("@stamp", stampEntity);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        transaction = new TransactionModel
                        {
                            StampEntity = reader["stamp_entity"].ToString(),
                            Description = reader["description"].ToString(),
                            Amount = Convert.ToDecimal(reader["amount"]),
                            Date = Convert.ToDateTime(reader["date"]),
                            Category = reader["category"].ToString(),
                            PaymentMethod = reader["paymentMethod"].ToString(),
                            Recipient = reader["recipient"].ToString(),
                            Status = reader["status"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            UpdatedAt = Convert.ToDateTime(reader["updated_at"])
                        };
                    }
                }
            }
            return transaction;
        }

        public async Task AddAsync(TransactionModel transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO Transactions 
                          (stamp_entity, description, amount, date, category, paymentMethod, recipient, status, created_at, updated_at)
                          VALUES (@stampEntity, @description, @amount, @date, @category, @paymentMethod, @recipient, @status, GETDATE(), GETDATE());";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@stampEntity", transaction.StampEntity);
                command.Parameters.AddWithValue("@description", transaction.Description);
                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@date", transaction.Date);
                command.Parameters.AddWithValue("@category", transaction.Category);
                command.Parameters.AddWithValue("@paymentMethod", transaction.PaymentMethod);
                command.Parameters.AddWithValue("@recipient", transaction.Recipient);
                command.Parameters.AddWithValue("@status", transaction.Status);

                await command.ExecuteScalarAsync();
            }
        }

        public async Task UpdateAsync(TransactionModel transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
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
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@description", transaction.Description);
                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@date", transaction.Date);
                command.Parameters.AddWithValue("@category", transaction.Category);
                command.Parameters.AddWithValue("@paymentMethod", transaction.PaymentMethod);
                command.Parameters.AddWithValue("@recipient", transaction.Recipient);
                command.Parameters.AddWithValue("@status", transaction.Status);
                command.Parameters.AddWithValue("@stamp", transaction.StampEntity);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(string stampEntity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("DELETE FROM Transactions WHERE stamp_entity = @stamp", connection);
                command.Parameters.AddWithValue("@stamp", stampEntity);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

}
