using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models
{
    public class PasswordResetRequestModel : BaseEntity
    {
        public string Token { get; set; }
        public string UserStamp { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Used { get; set; }

        public PasswordResetRequestModel(string userStamp, string token, DateTime expiresAt, bool used) : base(null)
        {
            UserStamp = userStamp;
            Token = token;
            ExpiresAt = expiresAt;
            Used = used;
        }

        public PasswordResetRequestModel(DataRow row)
        {
            UserStamp = row["user_stamp"].ToString();
            StampEntity = row["stamp_entity"].ToString();
            Token = row["token"].ToString();
            CreatedAt = DateTime.Parse(row["created_at"].ToString());
            ExpiresAt = DateTime.Parse(row["expires_at"].ToString());
            Used = Convert.ToBoolean(row["used"]);
        }

        public PasswordResetRequestModel(object? row)
        {
            if (row is IDictionary<string, object> dictRow)
            {
                UserStamp = dictRow.ContainsKey("user_stamp") ? dictRow["user_stamp"]?.ToString() ?? string.Empty : string.Empty;
                StampEntity = dictRow.ContainsKey("stamp_entity") ? dictRow["stamp_entity"]?.ToString() ?? string.Empty : string.Empty;
                Token = dictRow.ContainsKey("token") ? dictRow["token"]?.ToString() ?? string.Empty : string.Empty;
                CreatedAt = dictRow.ContainsKey("created_at") ? DateTime.Parse(dictRow["created_at"]?.ToString() ?? DateTime.UtcNow.ToString()) : DateTime.UtcNow;
                ExpiresAt = dictRow.ContainsKey("expires_at") ? DateTime.Parse(dictRow["expires_at"]?.ToString() ?? DateTime.MinValue.ToString()) : DateTime.MinValue;
                Used = dictRow.ContainsKey("used") && Convert.ToBoolean(dictRow["used"] ?? false);
            }
            else
            {
                throw new ArgumentException("O objeto fornecido não é compatível. Deve ser DataRow ou IDictionary<string, object>.");
            }

        }
    }

}
