using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TransactionModel>> GetTransactionsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<TransactionModel> GetTransactionByStampEntityAsync(string stampEntity)
        {
            return await _repository.GetByStampEntityAsync(stampEntity);
        }

        public async Task AddTransactionAsync(TransactionModel transaction)
        {
            // Ajusta o sinal do valor consoante a categoria
            if (transaction.Category == "expense")
            {
                transaction.Amount = -Math.Abs(transaction.Amount);
            }
            else
            {
                transaction.Amount = Math.Abs(transaction.Amount);
            }

            await _repository.AddAsync(transaction);
        }

        public async Task UpdateTransactionAsync(string stampEntity, TransactionModel updatedTransaction)
        {
            var transaction = await _repository.GetByStampEntityAsync(stampEntity);
            if (transaction == null)
                throw new Exception("Transacção não encontrada.");

            transaction.Description = updatedTransaction.Description;
            transaction.Date = updatedTransaction.Date;
            transaction.Category = updatedTransaction.Category;
            transaction.PaymentMethod = updatedTransaction.PaymentMethod;
            transaction.Recipient = updatedTransaction.Recipient;
            transaction.Status = updatedTransaction.Status;
            transaction.Amount = updatedTransaction.Category == "expense"
                                    ? -Math.Abs(updatedTransaction.Amount)
                                    : Math.Abs(updatedTransaction.Amount);

            await _repository.UpdateAsync(transaction);
        }

        public async Task DeleteTransactionAsync(string stampEntity)
        {
            await _repository.DeleteAsync(stampEntity);
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            var transactions = await _repository.GetAllAsync();
            return transactions.Sum(t => t.Amount);
        }

        public async Task<decimal> GetTotalIncomeAsync()
        {
            var transactions = await _repository.GetAllAsync();
            return transactions.Where(t => t.Category == "income").Sum(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpensesAsync()
        {
            var transactions = await _repository.GetAllAsync();
            return transactions.Where(t => t.Category == "expense").Sum(t => t.Amount);
        }
    }

}
