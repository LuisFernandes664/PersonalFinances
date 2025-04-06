using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Transaction
{
    public class RecurringTransactionRepository : IRecurringTransactionRepository
    {
        private readonly DatabaseContext _dbContext;

        public RecurringTransactionRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<RecurringTransactionModel>> GetAllByUserAsync(string userId)
        {
            var query = "SELECT * FROM RecurringTransactions WHERE user_id = @userId";
            var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            return result.Rows.Cast<DataRow>().Select(row => new RecurringTransactionModel(row)).ToList();
        }

        public async Task<RecurringTransactionModel> GetByIdAsync(string id)
        {
            var query = "SELECT * FROM RecurringTransactions WHERE stamp_entity = @id";
            var parameters = new List<SqlParameter> { new SqlParameter("@id", id) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            return result.Rows.Count > 0 ? new RecurringTransactionModel(result.Rows[0]) : null;
        }

        public async Task AddAsync(RecurringTransactionModel transaction)
        {
            var query = @"
                INSERT INTO RecurringTransactions (
                    stamp_entity, user_id, description, amount, category, payment_method, recipient,
                    recurrence_type, recurrence_interval, start_date, end_date, is_active,
                    last_processed_date, created_at, updated_at
                ) VALUES (
                    @stampEntity, @userId, @description, @amount, @category, @paymentMethod, @recipient,
                    @recurrenceType, @recurrenceInterval, @startDate, @endDate, @isActive,
                    @lastProcessedDate, @createdAt, @updatedAt
                )";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", transaction.StampEntity),
                new SqlParameter("@userId", transaction.UserId),
                new SqlParameter("@description", transaction.Description),
                new SqlParameter("@amount", transaction.Amount),
                new SqlParameter("@category", transaction.Category),
                new SqlParameter("@paymentMethod", transaction.PaymentMethod),
                new SqlParameter("@recipient", transaction.Recipient),
                new SqlParameter("@recurrenceType", (int)transaction.RecurrenceType),
                new SqlParameter("@recurrenceInterval", transaction.RecurrenceInterval),
                new SqlParameter("@startDate", transaction.StartDate),
                new SqlParameter("@endDate", transaction.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@isActive", transaction.IsActive),
                new SqlParameter("@lastProcessedDate", transaction.LastProcessedDate ?? (object)DBNull.Value),
                new SqlParameter("@createdAt", transaction.CreatedAt),
                new SqlParameter("@updatedAt", transaction.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task UpdateAsync(RecurringTransactionModel transaction)
        {
            var query = @"
                UPDATE RecurringTransactions SET
                    description = @description,
                    amount = @amount,
                    category = @category,
                    payment_method = @paymentMethod,
                    recipient = @recipient,
                    recurrence_type = @recurrenceType,
                    recurrence_interval = @recurrenceInterval,
                    start_date = @startDate,
                    end_date = @endDate,
                    is_active = @isActive,
                    last_processed_date = @lastProcessedDate,
                    updated_at = @updatedAt
                WHERE stamp_entity = @stampEntity";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", transaction.StampEntity),
                new SqlParameter("@description", transaction.Description),
                new SqlParameter("@amount", transaction.Amount),
                new SqlParameter("@category", transaction.Category),
                new SqlParameter("@paymentMethod", transaction.PaymentMethod),
                new SqlParameter("@recipient", transaction.Recipient),
                new SqlParameter("@recurrenceType", (int)transaction.RecurrenceType),
                new SqlParameter("@recurrenceInterval", transaction.RecurrenceInterval),
                new SqlParameter("@startDate", transaction.StartDate),
                new SqlParameter("@endDate", transaction.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@isActive", transaction.IsActive),
                new SqlParameter("@lastProcessedDate", transaction.LastProcessedDate ?? (object)DBNull.Value),
                new SqlParameter("@updatedAt", DateTime.UtcNow)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteAsync(string id)
        {
            var query = "DELETE FROM RecurringTransactions WHERE stamp_entity = @id";
            var parameters = new List<SqlParameter> { new SqlParameter("@id", id) };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<IEnumerable<RecurringTransactionModel>> GetDueTransactionsAsync()
        {
            var query = @"
                SELECT * FROM RecurringTransactions
                WHERE is_active = 1
                AND (
                    last_processed_date IS NULL
                    OR
                    (
                        recurrence_type = 0 -- Daily
                        AND DATEDIFF(day, last_processed_date, GETDATE()) >= recurrence_interval
                    )
                    OR
                    (
                        recurrence_type = 1 -- Weekly
                        AND DATEDIFF(week, last_processed_date, GETDATE()) >= recurrence_interval
                    )
                    OR
                    (
                        recurrence_type = 2 -- Monthly
                        AND DATEDIFF(month, last_processed_date, GETDATE()) >= recurrence_interval
                    )
                    OR
                    (
                        recurrence_type = 3 -- Yearly
                        AND DATEDIFF(year, last_processed_date, GETDATE()) >= recurrence_interval
                    )
                )
                AND (end_date IS NULL OR end_date >= GETDATE())";

            var result = await SQLHelper.ExecuteQueryAsync(query);
            return result.Rows.Cast<DataRow>().Select(row => new RecurringTransactionModel(row)).ToList();
        }

        public async Task UpdateLastProcessedDateAsync(string id, DateTime date)
        {
            var query = @"
                UPDATE RecurringTransactions
                SET last_processed_date = @date, updated_at = GETDATE()
                WHERE stamp_entity = @id";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@id", id),
                new SqlParameter("@date", date)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }
    }
}