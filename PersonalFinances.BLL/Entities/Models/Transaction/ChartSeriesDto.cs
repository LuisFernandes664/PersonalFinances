using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Transaction
{
    public class ChartSeriesDto
    {
        public string Name { get; set; }
        public List<decimal> Data { get; set; }
        public List<string> Categories { get; set; }
    }
}
