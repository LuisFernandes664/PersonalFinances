using PersonalFinances.BLL.Entities.Models.Analytics;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.SavingPlan.Budget
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repository;

        public BudgetService(IBudgetRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsByUserAsync(string userId) => await _repository.GetBudgetsByUserAsync(userId);

        public async Task<BudgetModel> GetBudgetByIdAsync(string budgetId) => await _repository.GetBudgetByIdAsync(budgetId);

        public async Task<decimal> GetSpentAmountByBudget(string budgetId) => await _repository.GetSpentAmountByBudget(budgetId);

        public async Task CreateBudgetAsync(BudgetModel budget) => await _repository.CreateBudgetAsync(budget);

        public async Task UpdateBudgetAsync(BudgetModel budget) => await _repository.UpdateBudgetAsync(budget);

        public async Task DeleteBudgetAsync(string budgetId) => await _repository.DeleteBudgetAsync(budgetId);

        public async Task<IEnumerable<BudgetHistoryModel>> GetBudgetHistoryAsync(string budgetId) => await _repository.GetBudgetHistoryAsync(budgetId);

        public async Task AddBudgetHistoryAsync(string budgetId, string transactionId, decimal valorGasto) => await _repository.AddBudgetHistoryAsync(budgetId, transactionId, valorGasto);

        public async Task UpdateBudgetSpentAmount(string budgetId) => await _repository.UpdateBudgetSpentAmount(budgetId);

        public async Task<BudgetAnalyticsModel> GetBudgetAnalyticsAsync(string budgetId)
        {
            // Obter orçamento
            var budget = await _repository.GetBudgetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new ArgumentException("Orçamento não encontrado", nameof(budgetId));
            }

            // Obter valor gasto
            var spentAmount = await _repository.GetSpentAmountByBudget(budgetId);

            // Calcular valores derivados
            var remainingAmount = budget.ValorOrcado - spentAmount;
            var percentUsed = budget.ValorOrcado > 0 ? (spentAmount / budget.ValorOrcado) * 100m : 0m;

            // Calcular dias restantes
            var today = DateTime.Today;
            var totalDays = (budget.DataFim - budget.DataInicio).TotalDays;
            var elapsedDays = (today - budget.DataInicio).TotalDays;
            elapsedDays = Math.Max(0, Math.Min(elapsedDays, totalDays)); // Limitar entre 0 e totalDays
            var remainingDays = (int)Math.Max(0, totalDays - elapsedDays);

            // Calcular média diária de gastos
            var avgDailySpending = elapsedDays > 0 ? spentAmount / (decimal)elapsedDays : 0m;

            // Projetar gasto final
            var projectedEndAmount = spentAmount + (avgDailySpending * remainingDays);

            // Determinar status
            string status;
            if (percentUsed <= 85m)
                status = "Em dia";
            else if (percentUsed <= 100m)
                status = "Risco";
            else
                status = "Ultrapassado";

            // Obter tendência de gastos (últimos 30 dias ou desde o início do orçamento)
            var trendStartDate = budget.DataInicio > today.AddDays(-30) ? budget.DataInicio : today.AddDays(-30);
            var trendData = await GetSpendingTrendAsync(budgetId, trendStartDate, today);

            // Criar e retornar o modelo de análise
            var categoryName = await _repository.GetCategoryNameById(budget.CategoryId);

            return new BudgetAnalyticsModel
            {
                BudgetId = budgetId,
                CategoryName = categoryName,
                BudgetAmount = budget.ValorOrcado,
                SpentAmount = spentAmount,
                RemainingAmount = remainingAmount,
                PercentUsed = percentUsed,
                ProjectedEndAmount = projectedEndAmount,
                IsOverBudget = spentAmount > budget.ValorOrcado,
                AverageDailySpending = avgDailySpending,
                RemainingDays = remainingDays,
                Status = status,
                SpendingTrend = trendData
            };
        }

        public async Task<IEnumerable<BudgetCategoryAnalyticsModel>> GetCategoryAnalyticsAsync(string userId)
        {
            // Obter todos os orçamentos do usuário
            var budgets = await _repository.GetBudgetsByUserAsync(userId);
            var result = new List<BudgetCategoryAnalyticsModel>();

            // Agrupar orçamentos por categoria
            var budgetsByCategory = budgets.GroupBy(b => b.CategoryId);

            foreach (var categoryGroup in budgetsByCategory)
            {
                var categoryId = categoryGroup.Key;
                var categoryName = await _repository.GetCategoryNameById(categoryId);

                decimal totalBudgeted = 0;
                decimal totalSpent = 0;

                foreach (var budget in categoryGroup)
                {
                    totalBudgeted += budget.ValorOrcado;
                    totalSpent += await _repository.GetSpentAmountByBudget(budget.StampEntity);
                }

                var percentUsed = totalBudgeted > 0 ? (totalSpent / totalBudgeted) * 100m : 0m;

                // Obter dados históricos dos últimos 6 meses
                var historicalData = await _repository.GetHistoricalSpendingByCategory(categoryId, 6);

                // Converter para o formato esperado
                var categoryHistoricalData = new List<BudgetCategoryHistoricalData>();
                foreach (var dataPoint in historicalData)
                {
                    // Encontrar orçamento do mês correspondente, se existir
                    var monthBudgets = budgets.Where(b =>
                        b.CategoryId == categoryId &&
                        b.DataInicio.Year == dataPoint.Month.Year &&
                        b.DataInicio.Month == dataPoint.Month.Month);

                    decimal monthBudgetedAmount = monthBudgets.Sum(b => b.ValorOrcado);

                    var variance = monthBudgetedAmount > 0
                        ? ((dataPoint.Amount - monthBudgetedAmount) / monthBudgetedAmount) * 100m
                        : 0m;

                    categoryHistoricalData.Add(new BudgetCategoryHistoricalData
                    {
                        MonthYear = dataPoint.Month.ToString("MMM yyyy"),
                        BudgetedAmount = monthBudgetedAmount,
                        SpentAmount = dataPoint.Amount,
                        VariancePercentage = variance
                    });
                }

                result.Add(new BudgetCategoryAnalyticsModel
                {
                    CategoryId = categoryId,
                    CategoryName = categoryName,
                    TotalBudgeted = totalBudgeted,
                    TotalSpent = totalSpent,
                    PercentUsed = percentUsed,
                    HistoricalData = categoryHistoricalData
                });
            }

            return result;
        }

        public async Task<BudgetForecastModel> GetBudgetForecastAsync(string budgetId, int monthsAhead)
        {
            // Obter o orçamento
            var budget = await _repository.GetBudgetByIdAsync(budgetId);
            if (budget == null)
            {
                throw new ArgumentException("Orçamento não encontrado", nameof(budgetId));
            }

            // Obter dados históricos para esta categoria
            var historicalData = await _repository.GetHistoricalSpendingByCategory(budget.CategoryId, 6);

            // Calcular média mensal de gastos
            var avgMonthlySpending = historicalData.Any()
                ? historicalData.Average(m => m.Amount)
                : 0m;

            // Calcular tendência de crescimento usando regressão linear
            double slope = CalculateGrowthTrend(historicalData);

            // Criar pontos de dados de previsão
            var forecastData = new List<ForecastDataPoint>();
            var currentDate = DateTime.Now;

            for (int i = 1; i <= monthsAhead; i++)
            {
                var forecastDate = currentDate.AddMonths(i);
                var forecastAmount = avgMonthlySpending + (decimal)(slope * i);
                forecastAmount = Math.Max(0, forecastAmount); // Garantir que não seja negativo

                forecastData.Add(new ForecastDataPoint
                {
                    Date = forecastDate,
                    Amount = forecastAmount,
                    IsOverBudget = forecastAmount > budget.ValorOrcado
                });
            }

            // Calcular gasto total previsto
            var forecastedTotalSpending = forecastData.Sum(d => d.Amount);

            // Obter nome da categoria
            var categoryName = await _repository.GetCategoryNameById(budget.CategoryId);

            return new BudgetForecastModel
            {
                BudgetId = budgetId,
                CategoryName = categoryName,
                BudgetAmount = budget.ValorOrcado,
                ForecastData = forecastData,
                ForecastedTotalSpending = forecastedTotalSpending,
                MonthsAhead = monthsAhead
            };
        }

        // Método auxiliar para calcular tendência de crescimento
        private double CalculateGrowthTrend(List<HistoricalSpendingModel> data)
        {
            // Se não houver dados suficientes, retornar 0 (sem crescimento)
            if (data.Count < 2) return 0;

            int n = data.Count;
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            // Regressão linear usando método dos mínimos quadrados
            for (int i = 0; i < n; i++)
            {
                double x = i;
                double y = (double)data[i].Amount;

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            // Fórmula para inclinação: (n*∑xy - ∑x*∑y) / (n*∑x² - (∑x)²)
            double slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }

        // Método auxiliar para obter a tendência de gastos diários
        private async Task<List<SpendingTrendDataPoint>> GetSpendingTrendAsync(string budgetId, DateTime startDate, DateTime endDate)
        {
            // Este método normalmente buscaria transações específicas do orçamento por dia
            // Para simplificar, vamos implementar uma versão básica

            var result = new List<SpendingTrendDataPoint>();

            // Obter o orçamento e seu valor
            var budget = await _repository.GetBudgetByIdAsync(budgetId);

            // Gerar pontos de dados para cada dia no intervalo
            decimal cumulativeAmount = 0;

            // Na implementação real, você buscaria transações reais do banco de dados
            // e agregaria por dia. Aqui estamos simplificando.
            var currentDate = startDate.Date;
            while (currentDate <= endDate.Date)
            {
                // Aqui você obteria o valor real gasto neste dia
                // Por exemplo: var dailyAmount = await _repository.GetDailySpendingForBudget(budgetId, currentDate);

                // Para este exemplo, vamos usar um valor aleatório
                var random = new Random();
                decimal dailyAmount = (decimal)(random.NextDouble() * (double)budget.ValorOrcado * 0.05);

                cumulativeAmount += dailyAmount;

                result.Add(new SpendingTrendDataPoint
                {
                    Date = currentDate,
                    Amount = dailyAmount,
                    CumulativeAmount = cumulativeAmount,
                    BudgetPercentage = budget.ValorOrcado > 0 ? (cumulativeAmount / budget.ValorOrcado) * 100m : 0m
                });

                currentDate = currentDate.AddDays(1);
            }

            return result;
        }

    }
}
