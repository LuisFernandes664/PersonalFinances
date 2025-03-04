using System;
using System.Data;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    /// <summary>
    /// Registra o progresso da meta ao longo do tempo.
    /// </summary>
    public class GoalProgressModel : BaseEntity
    {
        /// <summary>
        /// Identificador da meta associada a este progresso.
        /// </summary>
        public string GoalId { get; set; }

        /// <summary>
        /// Valor acumulado da meta no momento do registro.
        /// </summary>
        public decimal ValorAtual { get; set; }

        /// <summary>
        /// Data do registro do progresso.
        /// </summary>
        public DateTime DataRegistro { get; set; }

        public GoalProgressModel() : base() 
        {
            GoalId = string.Empty;
            ValorAtual = 0;
            DataRegistro = DateTime.Now;
        }

        public GoalProgressModel(DataRow row) : base(row)
        {
            GoalId = row.Field<string>("goal_id") ?? string.Empty;
            ValorAtual = row.Field<decimal?>("valor_atual") ?? 0;
            DataRegistro = row.Field<DateTime?>("data_registro") ?? DateTime.Now;
        }
    }
}
