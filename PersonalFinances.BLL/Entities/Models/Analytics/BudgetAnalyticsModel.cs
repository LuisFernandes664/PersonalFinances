using System;
using System.Collections.Generic;

namespace PersonalFinances.BLL.Entities.Models.Analytics
{
    /// <summary>
    /// Representa dados analíticos de um orçamento específico
    /// </summary>
    public class BudgetAnalyticsModel
    {
        public string BudgetId { get; set; }
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal PercentUsed { get; set; }
        public decimal ProjectedEndAmount { get; set; }
        public bool IsOverBudget { get; set; }
        public decimal AverageDailySpending { get; set; }
        public int RemainingDays { get; set; }
        public string Status { get; set; } // "Em dia", "Risco", "Ultrapassado"
        public List<SpendingTrendDataPoint> SpendingTrend { get; set; }

        public BudgetAnalyticsModel()
        {
            SpendingTrend = new List<SpendingTrendDataPoint>();
        }
    }

    /// <summary>
    /// Representa um ponto de dados na tendência de gastos do orçamento
    /// </summary>
    public class SpendingTrendDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal CumulativeAmount { get; set; }
        public decimal BudgetPercentage { get; set; }
    }

    /// <summary>
    /// Representa análise por categoria de despesa
    /// </summary>
    public class BudgetCategoryAnalyticsModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal PercentUsed { get; set; }
        public List<BudgetCategoryHistoricalData> HistoricalData { get; set; }

        public BudgetCategoryAnalyticsModel()
        {
            HistoricalData = new List<BudgetCategoryHistoricalData>();
        }
    }

    /// <summary>
    /// Representa dados históricos de uma categoria de orçamento
    /// </summary>
    public class BudgetCategoryHistoricalData
    {
        public string MonthYear { get; set; }
        public decimal BudgetedAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal VariancePercentage { get; set; }
    }

    /// <summary>
    /// Modelo para previsão de gastos futuros com orçamento
    /// </summary>
    public class BudgetForecastModel
    {
        public string BudgetId { get; set; }
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal ForecastedTotalSpending { get; set; }
        public int MonthsAhead { get; set; }
        public List<ForecastDataPoint> ForecastData { get; set; }

        public BudgetForecastModel()
        {
            ForecastData = new List<ForecastDataPoint>();
        }
    }

    /// <summary>
    /// Representa um ponto de dados na previsão de gastos
    /// </summary>
    public class ForecastDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool IsOverBudget { get; set; }
    }

    /// <summary>
    /// Modelo para representar dados históricos de gastos
    /// </summary>
    public class HistoricalSpendingModel
    {
        public DateTime Month { get; set; }
        public decimal Amount { get; set; }
    }
}