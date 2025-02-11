using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.DB
{
    public class ColumnDefinition
    {
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }

        // Novos campos para IsForeignKey
        public bool IsForeignKey { get; set; }
        public string ForeignKeyTable { get; set; }
        public string ForeignKeyColumn { get; set; }

        // Aumenta eficiencia de selects, joins etc.. lentidao na insersao + updates
        public bool HasIndex { get; set; } 
    }

}
