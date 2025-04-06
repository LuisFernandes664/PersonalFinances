using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Entities.Models.Transaction;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface ITagService
    {
        Task<IEnumerable<TagModel>> GetUserTagsAsync(string userId);
        Task<TagModel> GetTagByIdAsync(string tagId);
        Task<TagModel> CreateTagAsync(TagModel tag);
        Task UpdateTagAsync(TagModel tag);
        Task DeleteTagAsync(string tagId);
        Task AddTagToTransactionAsync(string transactionId, string tagId);
        Task RemoveTagFromTransactionAsync(string transactionId, string tagId);
        Task<IEnumerable<TransactionModel>> GetTransactionsByTagAsync(string tagId);
        Task LoadTagsForTransactionAsync(TransactionModel transaction);
    }
}
