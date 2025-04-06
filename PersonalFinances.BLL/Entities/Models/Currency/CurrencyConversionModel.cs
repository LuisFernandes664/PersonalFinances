using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models.Currency
{
    public class CurrencyConversionModel : BaseEntity
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime FetchedAt { get; set; }

        public CurrencyConversionModel() : base()
        {
            FromCurrency = "EUR";
            ToCurrency = "EUR";
            Rate = 1m;
            FetchedAt = DateTime.UtcNow;
        }

        public CurrencyConversionModel(DataRow row) : base(row)
        {
            FromCurrency = row.Field<string>("from_currency") ?? "EUR";
            ToCurrency = row.Field<string>("to_currency") ?? "EUR";
            Rate = row.Field<decimal>("rate");
            FetchedAt = row.Field<DateTime>("fetched_at");
        }
    }

}
