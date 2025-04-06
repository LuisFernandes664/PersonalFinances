using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Transaction
{
    public class TagModel : BaseEntity
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public TagModel() : base()
        {
            UserId = string.Empty;
            Name = string.Empty;
            Color = "#3498db";
        }

        public TagModel(DataRow row) : base(row)
        {
            UserId = row.Field<string>("user_id") ?? string.Empty;
            Name = row.Field<string>("name") ?? string.Empty;
            Color = row.Field<string>("color") ?? "#3498db";
        }
    }

    public class TransactionTagModel : BaseEntity
    {
        public string TransactionId { get; set; }
        public string TagId { get; set; }

        public TransactionTagModel() : base()
        {
            TransactionId = string.Empty;
            TagId = string.Empty;
        }

        public TransactionTagModel(DataRow row) : base(row)
        {
            TransactionId = row.Field<string>("transaction_id") ?? string.Empty;
            TagId = row.Field<string>("tag_id") ?? string.Empty;
        }
    }

}
