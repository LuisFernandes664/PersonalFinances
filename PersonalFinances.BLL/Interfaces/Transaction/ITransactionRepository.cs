using PersonalFinances.BLL.Entities.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionModel>> GetAllAsync();
        Task<TransactionModel> GetByStampEntityAsync(string stampEntity);
        Task AddAsync(TransactionModel transaction);
        Task UpdateAsync(TransactionModel transaction);
        Task DeleteAsync(string stampEntity);
    }

}
