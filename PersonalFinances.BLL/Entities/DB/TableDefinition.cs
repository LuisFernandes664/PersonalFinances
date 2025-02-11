using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.DB
{
    public class TableDefinition
    {
        public string TableName { get; set; }
        public Dictionary<string, ColumnDefinition> Columns { get; set; }
    }
}
