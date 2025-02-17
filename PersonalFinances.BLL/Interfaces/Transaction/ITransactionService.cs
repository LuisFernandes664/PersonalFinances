using PersonalFinances.BLL.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionModel>> GetTransactionsAsync();
        Task<TransactionModel> GetTransactionByStampEntityAsync(string stampEntity);
        Task AddTransactionAsync(TransactionModel transaction);
        Task UpdateTransactionAsync(string stampEntity, TransactionModel transaction);
        Task DeleteTransactionAsync(string stampEntity);
        Task<decimal> GetTotalBalanceAsync();
        Task<decimal> GetTotalIncomeAsync();
        Task<decimal> GetTotalExpensesAsync();
    }

}
