using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Transaction
{
    public class RecurringTransactionModel : BaseEntity
    {
        public string UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }
        public string Recipient { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public int RecurrenceInterval { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastProcessedDate { get; set; }

        public RecurringTransactionModel() : base()
        {
            UserId = string.Empty;
            Description = string.Empty;
            PaymentMethod = string.Empty;
            Recipient = string.Empty;
            IsActive = true;
        }

        public RecurringTransactionModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            Description = row.Field<string>("description") ?? string.Empty;
            Amount = row.Field<decimal>("amount");
            Category = row.Field<string>("category") ?? string.Empty;
            PaymentMethod = row.Field<string>("payment_method") ?? string.Empty;
            Recipient = row.Field<string>("recipient") ?? string.Empty;
            RecurrenceType = (RecurrenceType)row.Field<int>("recurrence_type");
            RecurrenceInterval = row.Field<int>("recurrence_interval");
            StartDate = row.Field<DateTime>("start_date");
            EndDate = row.Field<DateTime?>("end_date");
            IsActive = row.Field<bool>("is_active");
            LastProcessedDate = row.Field<DateTime?>("last_processed_date");
        }
    }

    public enum RecurrenceType
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Yearly = 3
    }
}
