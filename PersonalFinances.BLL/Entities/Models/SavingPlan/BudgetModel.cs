using System;
using System.Data;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    /// <summary>
    /// Representa um orçamento financeiro definido pelo utilizador.
    /// </summary>
    public class BudgetModel : BaseEntity
    {
        /// <summary>
        /// Identificador do utilizador ao qual o orçamento pertence.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Identificador da categoria do orçamento.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Valor máximo permitido para este orçamento.
        /// </summary>
        public decimal ValorOrcado { get; set; }

        /// <summary>
        /// Data de início do orçamento.
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data de término do orçamento.
        /// </summary>
        public DateTime DataFim { get; set; }

        public decimal PercentUsed { get; set; }
        public decimal ProjectedEndAmount { get; set; }
        public bool IsOverBudget { get; set; }
        public decimal AverageDailySpending { get; set; }

        public BudgetModel() : base() { 
            UserId = string.Empty;
            CategoryId = string.Empty;
            ValorOrcado = 0;
            DataInicio = DateTime.Now;
            DataFim = DateTime.Now;
        }

        public BudgetModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            CategoryId = row.Field<string>("category_id") ?? string.Empty;
            ValorOrcado = row.Field<decimal?>("valor_orcado") ?? 0;
            DataInicio = row.Field<DateTime?>("data_inicio") ?? DateTime.Now;
            DataFim = row.Field<DateTime?>("data_fim") ?? DateTime.Now;
        }
    }
}
