using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan
{
    public class BudgetModel : BaseEntity
    {
        public string UserId { get; set; }
        public string Categoria { get; set; }
        public decimal ValorOrcado { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }

}
