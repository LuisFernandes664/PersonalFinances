using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<DashboardTotalsModel> GetDashboardTotalsAsync()
        {
            var transactions = await _repository.GetAllAsync();

            // Obter a data atual
            var now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;
            int lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            int lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // Inicializa os acumuladores
            decimal currentIncome = 0m, currentExpenses = 0m;
            decimal lastIncome = 0m, lastExpenses = 0m;

            foreach (var t in transactions)
            {
                if (t.Date.Month == currentMonth && t.Date.Year == currentYear)
                {
                    if (t.Category == "income")
                        currentIncome += t.Amount;
                    else if (t.Category == "expense")
                        currentExpenses += t.Amount;
                }
                else if (t.Date.Month == lastMonth && t.Date.Year == lastMonthYear)
                {
                    if (t.Category == "income")
                        lastIncome += t.Amount;
                    else if (t.Category == "expense")
                        lastExpenses += t.Amount;
                }
            }

            // Cálculos para o mês atual
            var totalIncome = currentIncome;
            var totalExpenses = currentExpenses;
            var totalBalance = totalIncome - totalExpenses;

            // Cálculos para o mês passado
            var lastMonthBalance = lastIncome - lastExpenses;

            // Cálculo da variação do saldo
            decimal balanceVariation = 0;
            if (lastMonthBalance != 0)
            {
                balanceVariation = ((totalBalance - lastMonthBalance) / Math.Abs(lastMonthBalance)) * 100;
            }

            // Cálculo de savings (exemplo: 10% do totalIncome) e savingVariation (exemplo: 4% do savings)
            var savings = totalIncome * 0.1m;
            var savingVariation = savings * 0.04m;

            return new DashboardTotalsModel
            {
                TotalBalance = totalBalance,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                LastMonthBalance = lastMonthBalance,
                BalanceVariation = balanceVariation,
                Savings = savings,
                SavingVariation = savingVariation
            };
        }
    }
}
