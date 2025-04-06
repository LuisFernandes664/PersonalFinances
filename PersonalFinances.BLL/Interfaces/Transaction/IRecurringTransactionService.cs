using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinances.BLL.Entities.Models.Transaction;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface IRecurringTransactionService
    {
        Task<IEnumerable<RecurringTransactionModel>> GetUserRecurringTransactionsAsync(string userId);
        Task<RecurringTransactionModel> GetRecurringTransactionAsync(string id);
        Task CreateRecurringTransactionAsync(RecurringTransactionModel transaction);
        Task UpdateRecurringTransactionAsync(RecurringTransactionModel transaction);
        Task DeleteRecurringTransactionAsync(string id);
        Task ProcessDueRecurringTransactionsAsync();
    }
}
