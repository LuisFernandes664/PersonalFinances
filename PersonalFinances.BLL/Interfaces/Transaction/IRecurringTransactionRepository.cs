using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Entities.Models.Transaction;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface IRecurringTransactionRepository
    {
        Task<IEnumerable<RecurringTransactionModel>> GetAllByUserAsync(string userId);
        Task<RecurringTransactionModel> GetByIdAsync(string id);
        Task AddAsync(RecurringTransactionModel transaction);
        Task UpdateAsync(RecurringTransactionModel transaction);
        Task DeleteAsync(string id);
        Task<IEnumerable<RecurringTransactionModel>> GetDueTransactionsAsync();
        Task UpdateLastProcessedDateAsync(string id, DateTime date);
    }
}
