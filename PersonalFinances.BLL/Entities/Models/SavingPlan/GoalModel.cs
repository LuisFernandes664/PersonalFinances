using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    public class GoalModel : BaseEntity
    {
        public string UserId { get; set; }
        public string Descricao { get; set; }
        public decimal ValorAlvo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime DataLimite { get; set; }
    }

}
