using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Transaction
{
    public class ReceiptModel : BaseEntity
    {
        public string UserId { get; set; }
        public string ImagePath { get; set; }
        public string MerchantName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string TransactionId { get; set; }
        public bool IsProcessed { get; set; }
        public string ProcessingStatus { get; set; }
        public string ErrorMessage { get; set; }

        public string ImageBase64 { get; set; }
        public string ContentType { get; set; }

        public ReceiptModel() : base()
        {
            UserId = string.Empty;
            ImagePath = string.Empty;
            MerchantName = string.Empty;
            ProcessingStatus = "Pending";
            ImageBase64 = "";
        }

        public ReceiptModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            ImagePath = row.Field<string>("image_path") ?? string.Empty;
            MerchantName = row.Field<string>("merchant_name") ?? string.Empty;
            TotalAmount = row.Field<decimal?>("total_amount") ?? 0m;
            ReceiptDate = row.Field<DateTime?>("receipt_date") ?? DateTime.Now;
            TransactionId = row.Field<string>("transaction_id");
            IsProcessed = row.Field<bool>("is_processed");
            ProcessingStatus = row.Field<string>("processing_status") ?? "Pending";
            ErrorMessage = row.Field<string>("error_message");
            ImageBase64 = row.Field<string>("image_base64");
            ContentType = row.Field<string>("content_type");
        }
    }
}
