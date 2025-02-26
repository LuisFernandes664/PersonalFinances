using System;
using System.Data;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    /// <summary>
    /// Representa uma meta financeira definida pelo utilizador.
    /// </summary>
    public class GoalModel : BaseEntity
    {
        /// <summary>
        /// Identificador do utilizador ao qual a meta pertence.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Identificador da categoria da meta.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// Descrição da meta.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor total necessário para atingir a meta.
        /// </summary>
        public decimal ValorAlvo { get; set; }

        /// <summary>
        /// Data limite para atingir a meta.
        /// </summary>
        public DateTime DataLimite { get; set; }

        /// <summary>
        /// Valor acumulado até o momento, atualizado com base no GoalProgress.
        /// </summary>
        public decimal ValorAtual { get; set; }

        public GoalModel() : base() { }

        public GoalModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            CategoryId = row.Field<string>("category_id") ?? string.Empty;
            Descricao = row.Field<string>("descricao") ?? string.Empty;
            ValorAlvo = row.Field<decimal?>("valor_alvo") ?? 0;
            DataLimite = row.Field<DateTime?>("data_limite") ?? DateTime.Now;
            ValorAtual = row.Field<decimal?>("valor_atual") ?? 0;
        }
    }
}
