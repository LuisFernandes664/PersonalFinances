using System;
using System.Data;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    /// <summary>
    /// Registra os gastos associados a um orçamento.
    /// </summary>
    public class BudgetHistoryModel : BaseEntity
    {
        /// <summary>
        /// Identificador do orçamento associado ao gasto.
        /// </summary>
        public string BudgetId { get; set; }

        /// <summary>
        /// Identificador da transação vinculada ao orçamento.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Valor gasto nesta transação dentro do orçamento.
        /// </summary>
        public decimal ValorGasto { get; set; }

        /// <summary>
        /// Data do registro deste gasto no orçamento.
        /// </summary>
        public DateTime DataRegistro { get; set; }

        public BudgetHistoryModel() : base()
        {
            BudgetId = string.Empty;
            TransactionId = string.Empty;
        }

        public BudgetHistoryModel(DataRow row) : base(row)
        {
            BudgetId = row.Field<string>("budget_id") ?? string.Empty;
            TransactionId = row.Field<string>("transaction_id") ?? string.Empty;
            ValorGasto = row.Field<decimal?>("valor_gasto") ?? 0;
            DataRegistro = row.Field<DateTime?>("data_registro") ?? DateTime.Now;
        }
    }
}
