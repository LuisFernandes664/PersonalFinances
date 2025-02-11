using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Helpers
{
    public static class ModelsHelper
    {

        public static T IfExists<T>(DataRow row, string columnName, T defaultVal)
        {
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                return (T)Convert.ChangeType(row[columnName], typeof(T));
            }
            return defaultVal;
        }

    }
}
