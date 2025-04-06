using PersonalFinances.BLL.Entities.Models.Analytics;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Analytics;
using PersonalFinances.BLL.Interfaces.SavingPlan.Budget;
using PersonalFinances.BLL.Interfaces.SavingPlan.Goal;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Services.Analytics
{
    public class FinancialHealthService : IFinancialHealthService
    {
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;
        private readonly IGoalService _goalService;

        public FinancialHealthService(
            ITransactionService transactionService,
            IBudgetService budgetService,
            IGoalService goalService)
        {
            _transactionService = transactionService;
            _budgetService = budgetService;
            _goalService = goalService;
        }

        public async Task<FinancialHealthModel> CalculateFinancialHealthAsync(string userId)
        {
            // Obter dados financeiros para o usuário
            var totalIncome = await _transactionService.GetTotalIncomeAsync(userId);
            var totalExpenses = await _transactionService.GetTotalExpensesAsync(userId);
            var budgets = await _budgetService.GetBudgetsByUserAsync(userId);
            var transactions = await _transactionService.GetTransactionsAsync(userId);

            // Calcular taxa de poupança (% da renda economizada)
            decimal savingsRate = 0;
            if (totalIncome > 0)
            {
                savingsRate = ((totalIncome - Math.Abs(totalExpenses)) / totalIncome) * 100;
            }

            // Calcular pontuação de aderência ao orçamento
            int budgetAdherenceScore = await CalculateBudgetAdherenceScoreAsync(userId, budgets);

            // Calcular pontuação de padrões de gastos
            int spendingScore = CalculateSpendingScore(transactions);

            // Calcular pontuação de dívida
            int debtScore = CalculateDebtScore(transactions);

            // Calcular pontuação geral (média ponderada)
            int overallScore = (int)(0.35 * MapSavingsRateToScore(savingsRate) +
                                   0.25 * budgetAdherenceScore +
                                   0.25 * spendingScore +
                                   0.15 * debtScore);

            // Gerar recomendações
            var recommendations = GenerateRecommendations(
                savingsRate, budgetAdherenceScore, spendingScore, debtScore,
                totalIncome, totalExpenses, budgets, transactions);

            // Criar modelo de saúde financeira
            var healthModel = new FinancialHealthModel
            {
                UserId = userId,
                OverallScore = overallScore,
                SavingsScore = MapSavingsRateToScore(savingsRate),
                SpendingScore = spendingScore,
                DebtScore = debtScore,
                BudgetAdherenceScore = budgetAdherenceScore,
                Recommendations = recommendations,
                CalculatedAt = DateTime.UtcNow
            };

            // Salvar o modelo no banco de dados
            await SaveFinancialHealthAsync(healthModel);

            return healthModel;
        }

        public async Task<FinancialHealthModel> GetLatestFinancialHealthAsync(string userId)
        {
            var query = @"
                SELECT TOP 1 *
                FROM FinancialHealth
                WHERE user_id = @userId
                ORDER BY calculated_at DESC";

            var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
            {
                var healthModel = new FinancialHealthModel(result.Rows[0]);

                // Carregar recomendações
                var recQuery = @"
                    SELECT *
                    FROM FinancialHealthRecommendations
                    WHERE health_id = @healthId";

                var recParams = new List<SqlParameter> { new SqlParameter("@healthId", healthModel.StampEntity) };
                var recResult = await SQLHelper.ExecuteQueryAsync(recQuery, recParams);

                var recommendations = new List<FinancialHealthRecommendation>();
                foreach (System.Data.DataRow row in recResult.Rows)
                {
                    recommendations.Add(new FinancialHealthRecommendation(row));
                }

                healthModel.Recommendations = recommendations;
                return healthModel;
            }

            return null;
        }

        public async Task<IEnumerable<FinancialHealthModel>> GetFinancialHealthHistoryAsync(string userId, int months)
        {
            var query = @"
                SELECT *
                FROM FinancialHealth
                WHERE user_id = @userId
                AND calculated_at >= DATEADD(month, -@months, GETDATE())
                ORDER BY calculated_at";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@months", months)
            };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var history = new List<FinancialHealthModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                var healthModel = new FinancialHealthModel(row);

                // Não carregar recomendações para o histórico para economizar tempo
                healthModel.Recommendations = new List<FinancialHealthRecommendation>();

                history.Add(healthModel);
            }

            return history;
        }

        public async Task SaveFinancialHealthAsync(FinancialHealthModel healthModel)
        {
            // Salvar o modelo principal
            var query = @"
                INSERT INTO FinancialHealth (
                    stamp_entity, user_id, overall_score, savings_score, spending_score,
                    debt_score, budget_adherence_score, calculated_at, created_at, updated_at
                ) VALUES (
                    @stampEntity, @userId, @overallScore, @savingsScore, @spendingScore,
                    @debtScore, @budgetAdherenceScore, @calculatedAt, @createdAt, @updatedAt
                )";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", healthModel.StampEntity),
                new SqlParameter("@userId", healthModel.UserId),
                new SqlParameter("@overallScore", healthModel.OverallScore),
                new SqlParameter("@savingsScore", healthModel.SavingsScore),
                new SqlParameter("@spendingScore", healthModel.SpendingScore),
                new SqlParameter("@debtScore", healthModel.DebtScore),
                new SqlParameter("@budgetAdherenceScore", healthModel.BudgetAdherenceScore),
                new SqlParameter("@calculatedAt", healthModel.CalculatedAt),
                new SqlParameter("@createdAt", healthModel.CreatedAt),
                new SqlParameter("@updatedAt", healthModel.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);

            // Salvar as recomendações
            foreach (var recommendation in healthModel.Recommendations)
            {
                var recQuery = @"
                    INSERT INTO FinancialHealthRecommendations (
                        stamp_entity, health_id, category, description, action_item, priority_level, created_at
                    ) VALUES (
                        @stampEntity, @healthId, @category, @description, @actionItem, @priorityLevel, @createdAt
                    )";

                var recParams = new List<SqlParameter>
                {
                    new SqlParameter("@stampEntity", Guid.NewGuid().ToString()),
                    new SqlParameter("@healthId", healthModel.StampEntity),
                    new SqlParameter("@category", recommendation.Category),
                    new SqlParameter("@description", recommendation.Description),
                    new SqlParameter("@actionItem", recommendation.ActionItem),
                    new SqlParameter("@priorityLevel", recommendation.PriorityLevel),
                    new SqlParameter("@createdAt", DateTime.UtcNow)
                };

                await SQLHelper.ExecuteNonQueryAsync(recQuery, recParams);
            }
        }

        // Métodos auxiliares para cálculos de pontuação
        private int MapSavingsRateToScore(decimal savingsRate)
        {
            if (savingsRate >= 20) return 100;
            if (savingsRate >= 15) return 90;
            if (savingsRate >= 10) return 75;
            if (savingsRate >= 5) return 50;
            if (savingsRate > 0) return 30;
            return 0;
        }

        private async Task<int> CalculateBudgetAdherenceScoreAsync(string userId, IEnumerable<BLL.Entities.Models.SavingPlan.BudgetModel> budgets)
        {
            decimal totalBudgeted = 0;
            decimal totalOverspent = 0;

            foreach (var budget in budgets)
            {
                decimal spent = await _budgetService.GetSpentAmountByBudget(budget.StampEntity);
                totalBudgeted += budget.ValorOrcado;

                if (spent > budget.ValorOrcado)
                {
                    totalOverspent += (spent - budget.ValorOrcado);
                }
            }

            if (totalBudgeted == 0)
            {
                // Se não há orçamentos, a pontuação é média
                return 50;
            }

            decimal overBudgetPercentage = (totalOverspent / totalBudgeted) * 100;

            if (overBudgetPercentage == 0) return 100;
            if (overBudgetPercentage < 5) return 90;
            if (overBudgetPercentage < 10) return 80;
            if (overBudgetPercentage < 15) return 70;
            if (overBudgetPercentage < 20) return 60;
            if (overBudgetPercentage < 30) return 50;
            if (overBudgetPercentage < 40) return 40;
            if (overBudgetPercentage < 50) return 30;
            if (overBudgetPercentage < 75) return 20;
            return 10;
        }

        private int CalculateSpendingScore(IEnumerable<TransactionModel> transactions)
        {
            // Implementação básica - este método seria mais sofisticado em um sistema real
            // Aqui estamos dando uma pontuação baseada em uma distribuição de gastos saudável

            // Categorizar transações
            decimal totalExpenses = 0;
            decimal essentialExpenses = 0;
            decimal discretionaryExpenses = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Category == "expense")
                {
                    decimal amount = Math.Abs(transaction.Amount);
                    totalExpenses += amount;

                    // Categorizar como essencial ou discricionário (isso precisaria ser personalizado)
                    if (transaction.Description.Contains("aluguel") ||
                        transaction.Description.Contains("mercado") ||
                        transaction.Description.Contains("água") ||
                        transaction.Description.Contains("luz") ||
                        transaction.Description.Contains("internet") ||
                        transaction.Description.Contains("transporte") ||
                        transaction.Description.Contains("saúde"))
                    {
                        essentialExpenses += amount;
                    }
                    else
                    {
                        discretionaryExpenses += amount;
                    }
                }
            }

            if (totalExpenses == 0)
            {
                // Se não há despesas, a pontuação é média
                return 50;
            }

            // Calcular proporção de gastos discricionários
            decimal discretionaryRatio = discretionaryExpenses / totalExpenses;

            if (discretionaryRatio <= 0.2m) return 100; // Excelente proporção
            if (discretionaryRatio <= 0.3m) return 90;
            if (discretionaryRatio <= 0.4m) return 80;
            if (discretionaryRatio <= 0.5m) return 70;
            if (discretionaryRatio <= 0.6m) return 60;
            if (discretionaryRatio <= 0.7m) return 50;
            if (discretionaryRatio <= 0.8m) return 40;
            if (discretionaryRatio <= 0.9m) return 30;
            return 20; // Quase tudo é gasto discricionário
        }

        private int CalculateDebtScore(IEnumerable<TransactionModel> transactions)
        {
            // Implementação básica - este método seria mais sofisticado em um sistema real

            // Em um sistema real, você teria informações sobre dívidas
            // Aqui estamos dando uma pontuação padrão
            return 75;
        }

        private List<FinancialHealthRecommendation> GenerateRecommendations(
            decimal savingsRate, int budgetAdherenceScore, int spendingScore,
            int debtScore, decimal totalIncome, decimal totalExpenses,
            IEnumerable<BLL.Entities.Models.SavingPlan.BudgetModel> budgets,
            IEnumerable<TransactionModel> transactions)
        {
            var recommendations = new List<FinancialHealthRecommendation>();

            // Recomendações de poupança
            if (savingsRate < 10)
            {
                recommendations.Add(new FinancialHealthRecommendation
                {
                    Category = "Savings",
                    Description = "Sua taxa de poupança está abaixo do recomendado de 10%",
                    ActionItem = "Tente aumentar suas economias para pelo menos 10% da sua renda",
                    PriorityLevel = 1
                });
            }

            // Recomendações de orçamento
            if (budgetAdherenceScore < 70)
            {
                recommendations.Add(new FinancialHealthRecommendation
                {
                    Category = "Budget",
                    Description = "Você está gastando além do seu orçamento em algumas categorias",
                    ActionItem = "Revise seus orçamentos mensais e ajuste-os para valores mais realistas",
                    PriorityLevel = 2
                });
            }

            // Recomendações de gastos
            if (spendingScore < 60)
            {
                recommendations.Add(new FinancialHealthRecommendation
                {
                    Category = "Spending",
                    Description = "Seus gastos discricionários estão acima do ideal",
                    ActionItem = "Tente reduzir gastos não essenciais como entretenimento e refeições fora",
                    PriorityLevel = 2
                });
            }

            // Recomendações de dívida (placeholder)
            if (debtScore < 70)
            {
                recommendations.Add(new FinancialHealthRecommendation
                {
                    Category = "Debt",
                    Description = "Seu nível de dívida está acima do recomendado",
                    ActionItem = "Foque em pagar as dívidas de juros mais altos primeiro",
                    PriorityLevel = 1
                });
            }

            return recommendations;
        }
    }
}