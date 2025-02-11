using PersonalFinances.BLL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.DAL.Helpers
{
    public static class DateRangeHelper
    {
        /// <summary>
        /// Normaliza as datas do filtro para que:
        /// - Se existir uma data de início, usa-a diretamente.
        /// - Se existir uma data de fim, ajusta-a para o final desse dia.
        /// </summary>
        /// <param name="startDate">Data de início opcional.</param>
        /// <param name="endDate">Data de fim opcional.</param>
        /// <returns>Um DateRange com as datas normalizadas.</returns>
        public static DateRange Normalize(DateTime? startDate, DateTime? endDate)
        {
            var range = new DateRange();

            if (startDate.HasValue)
            {
                // Utiliza a data de início sem alteração
                range.Start = startDate.Value;
            }

            if (endDate.HasValue)
            {
                // Define o fim do dia para a data de fim:
                // Por exemplo, se endDate for 2025-02-01, define como 2025-02-01 23:59:59
                range.End = endDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            return range;
        }
    }


}
