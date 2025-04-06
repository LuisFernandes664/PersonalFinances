using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.Transaction
{
    public class TagService : ITagService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ITransactionService _transactionService;

        public TagService(DatabaseContext dbContext, ITransactionService transactionService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
        }

        public async Task<IEnumerable<TagModel>> GetUserTagsAsync(string userId)
        {
            var query = "SELECT * FROM Tags WHERE user_id = @userId";
            var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var tags = new List<TagModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                tags.Add(new TagModel(row));
            }

            return tags;
        }

        public async Task<TagModel> GetTagByIdAsync(string tagId)
        {
            var query = "SELECT * FROM Tags WHERE stamp_entity = @tagId";
            var parameters = new List<SqlParameter> { new SqlParameter("@tagId", tagId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);

            if (result.Rows.Count > 0)
            {
                return new TagModel(result.Rows[0]);
            }

            return null;
        }

        public async Task<TagModel> CreateTagAsync(TagModel tag)
        {
            tag.StampEntity = Guid.NewGuid().ToString();
            tag.CreatedAt = DateTime.UtcNow;
            tag.UpdatedAt = DateTime.UtcNow;

            var query = @"
                INSERT INTO Tags (stamp_entity, user_id, name, color, created_at, updated_at)
                VALUES (@stampEntity, @userId, @name, @color, @createdAt, @updatedAt)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", tag.StampEntity),
                new SqlParameter("@userId", tag.UserId),
                new SqlParameter("@name", tag.Name),
                new SqlParameter("@color", tag.Color),
                new SqlParameter("@createdAt", tag.CreatedAt),
                new SqlParameter("@updatedAt", tag.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);

            return tag;
        }

        public async Task UpdateTagAsync(TagModel tag)
        {
            tag.UpdatedAt = DateTime.UtcNow;

            var query = @"
                UPDATE Tags
                SET name = @name, color = @color, updated_at = @updatedAt
                WHERE stamp_entity = @stampEntity";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", tag.StampEntity),
                new SqlParameter("@name", tag.Name),
                new SqlParameter("@color", tag.Color),
                new SqlParameter("@updatedAt", tag.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task DeleteTagAsync(string tagId)
        {
            // Primeiro remover todas as relações com transações
            var query1 = "DELETE FROM TransactionTags WHERE tag_id = @tagId";
            var parameters1 = new List<SqlParameter> { new SqlParameter("@tagId", tagId) };

            await SQLHelper.ExecuteNonQueryAsync(query1, parameters1);

            // Depois remover a tag
            var query2 = "DELETE FROM Tags WHERE stamp_entity = @tagId";
            var parameters2 = new List<SqlParameter> { new SqlParameter("@tagId", tagId) };

            await SQLHelper.ExecuteNonQueryAsync(query2, parameters2);
        }

        public async Task AddTagToTransactionAsync(string transactionId, string tagId)
        {
            // Verificar se já existe essa relação
            var checkQuery = "SELECT COUNT(*) FROM TransactionTags WHERE transaction_id = @transactionId AND tag_id = @tagId";
            var checkParams = new List<SqlParameter>
            {
                new SqlParameter("@transactionId", transactionId),
                new SqlParameter("@tagId", tagId)
            };

            var count = Convert.ToInt32(await SQLHelper.ExecuteScalarAsync(checkQuery, checkParams));

            if (count == 0)
            {
                var query = @"
                    INSERT INTO TransactionTags (stamp_entity, transaction_id, tag_id, created_at)
                    VALUES (@stampEntity, @transactionId, @tagId, @createdAt)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@stampEntity", Guid.NewGuid().ToString()),
                    new SqlParameter("@transactionId", transactionId),
                    new SqlParameter("@tagId", tagId),
                    new SqlParameter("@createdAt", DateTime.UtcNow)
                };

                await SQLHelper.ExecuteNonQueryAsync(query, parameters);
            }
        }

        public async Task RemoveTagFromTransactionAsync(string transactionId, string tagId)
        {
            var query = "DELETE FROM TransactionTags WHERE transaction_id = @transactionId AND tag_id = @tagId";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@transactionId", transactionId),
                new SqlParameter("@tagId", tagId)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }

        public async Task<IEnumerable<TransactionModel>> GetTransactionsByTagAsync(string tagId)
        {
            var query = @"
                SELECT t.transaction_id
                FROM TransactionTags t
                WHERE t.tag_id = @tagId";

            var parameters = new List<SqlParameter> { new SqlParameter("@tagId", tagId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var transactions = new List<TransactionModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                var transactionId = row["transaction_id"].ToString();
                var transaction = await _transactionService.GetTransactionByStampEntityAsync(transactionId);

                if (transaction != null)
                {
                    transactions.Add(transaction);
                }
            }

            return transactions;
        }

        public async Task LoadTagsForTransactionAsync(TransactionModel transaction)
        {
            var query = @"
                SELECT t.*
                FROM Tags t
                INNER JOIN TransactionTags tt ON t.stamp_entity = tt.tag_id
                WHERE tt.transaction_id = @transactionId";

            var parameters = new List<SqlParameter> { new SqlParameter("@transactionId", transaction.StampEntity) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var tags = new List<TagModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                tags.Add(new TagModel(row));
            }

            // Adicionar as tags à propriedade Tags da transação (assuma que TransactionModel tem uma propriedade Tags)
            var transactionType = transaction.GetType();
            var tagsProperty = transactionType.GetProperty("Tags");

            if (tagsProperty != null)
            {
                tagsProperty.SetValue(transaction, tags);
            }
        }
    }
}