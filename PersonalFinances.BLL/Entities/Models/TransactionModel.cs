using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models
{
    public class TransactionModel : BaseEntity
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } // "income" ou "expense"
        public string PaymentMethod { get; set; } // "cash", "credit_card", "debit_card", "bank_transfer"
        public string Recipient { get; set; }
        public string Status { get; set; } // "pending", "confirmed", "cancelled"
    }

}
