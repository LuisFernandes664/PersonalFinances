using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities
{
    public abstract class BaseEntity
    {
        public string StampEntity { get; set; }

        /// <summary>
        /// Data e hora em que o utilizador foi criado.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Data e hora da última atualização dos dados do utilizador.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public BaseEntity()
        {
            StampEntity = GenerateStampEntity();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public BaseEntity(DataRow row)
        {
            StampEntity = row.Field<string>("stamp_entity") ?? string.Empty;
            CreatedAt = row.Field<DateTime?>("created_at") ?? DateTime.Now;
            UpdatedAt = CreatedAt = row.Field<DateTime?>("updated_at") ?? DateTime.Now;
        }

        public BaseEntity(object row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            dynamic dRow = row;
            StampEntity = dRow["stamp_entity"] ?? string.Empty;
            CreatedAt = dRow["created_at"] ?? DateTime.Now;
            UpdatedAt = dRow["updated_at"] ?? DateTime.Now;
        }


        public void UpdateDate()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        private string GenerateStampEntity() { return Guid.NewGuid().ToString(); }
    }
}
