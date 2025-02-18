using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public class DashboardTotalsModel
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal LastMonthBalance { get; set; }
        public decimal BalanceVariation { get; set; }
        public decimal Savings { get; set; }
        public decimal SavingVariation { get; set; }
    }

}
