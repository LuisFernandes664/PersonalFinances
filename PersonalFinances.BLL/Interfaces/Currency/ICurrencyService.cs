using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Currency
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency);
        Task<decimal> GetLatestExchangeRateAsync(string fromCurrency, string toCurrency);
        Task<IEnumerable<string>> GetAvailableCurrenciesAsync();
        Task UpdateExchangeRatesAsync();
    }

}
