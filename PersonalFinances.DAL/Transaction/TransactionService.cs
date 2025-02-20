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

        public async Task<ChartDataModel> GetChartDataAsync(string interval)
        {
            var transactions = await _repository.GetAllAsync();
            var now = DateTime.Now;
            var groupedData = new Dictionary<string, (decimal profits, decimal losses)>();

            Func<TransactionModel, string> keySelector;
            switch (interval.ToLower())
            {
                case "daily":
                    keySelector = t => t.Date.ToString("yyyy-MM-dd");
                    break;
                case "weekly":
                    keySelector = t => $"{t.Date.Year}-W{GetWeek(t.Date)}";
                    break;
                case "monthly":
                    keySelector = t => t.Date.ToString("yyyy-MM");
                    break;
                default:
                    throw new ArgumentException("Intervalo inválido. Use 'daily', 'weekly' ou 'monthly'.");
            }

            foreach (var t in transactions)
            {
                var key = keySelector(t);
                if (!groupedData.ContainsKey(key))
                    groupedData[key] = (0m, 0m);

                var (profits, losses) = groupedData[key];
                if (t.Category == "income")
                    profits += t.Amount;
                else if (t.Category == "expense")
                    losses += Math.Abs(t.Amount);
                groupedData[key] = (profits, losses);
            }

            var orderedKeys = groupedData.Keys.OrderBy(k => k).ToList();
            var profitsData = orderedKeys.Select(k => groupedData[k].profits).ToList();
            var lossesData = orderedKeys.Select(k => groupedData[k].losses).ToList();

            var series = new List<ChartSeriesModel>
            {
                new ChartSeriesModel { Name = "Lucros", Data = profitsData },
                new ChartSeriesModel { Name = "Perdas", Data = lossesData }
            };

            return new ChartDataModel
            {
                Series = series,
                Categories = orderedKeys
            };
        }

        // Método auxiliar para obter o número da semana
        private static int GetWeek(DateTime date)
        {
            var day = date.DayOfYear;
            var firstDay = new DateTime(date.Year, 1, 1);
            var weekOffset = (7 - (int)firstDay.DayOfWeek + 1) % 7;
            return (day + weekOffset) / 7 + 1;
        }
    }
}
