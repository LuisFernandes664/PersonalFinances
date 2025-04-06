using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinances.BLL.Interfaces.Currency;
using PersonalFinances.DAL.Helpers;

namespace PersonalFinances.DAL.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly DatabaseContext _dbContext;
        private readonly string _apiKey;

        public CurrencyService(
            HttpClient httpClient,
            IMemoryCache cache,
            DatabaseContext dbContext,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _dbContext = dbContext;
            _apiKey = configuration["ExchangeRateAPI:Key"] ?? string.Empty;
        }

        public async Task<decimal> ConvertAmountAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency)
                return amount;

            var rate = await GetLatestExchangeRateAsync(fromCurrency, toCurrency);
            return amount * rate;
        }

        public async Task<decimal> GetLatestExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            // Verificar cache primeiro
            string cacheKey = $"ExchangeRate_{fromCurrency}_{toCurrency}";
            if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
            {
                return cachedRate;
            }

            // Verificar banco de dados para taxas recentes (últimas 24 horas)
            var query = @"
                SELECT TOP 1 rate 
                FROM CurrencyConversions 
                WHERE from_currency = @fromCurrency 
                AND to_currency = @toCurrency 
                AND fetched_at > @cutoffDate 
                ORDER BY fetched_at DESC";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@fromCurrency", fromCurrency),
                new SqlParameter("@toCurrency", toCurrency),
                new SqlParameter("@cutoffDate", DateTime.UtcNow.AddHours(-24))
            };

            var result = await SQLHelper.ExecuteScalarAsync(query, parameters);
            if (result != null && result.Table.Columns.Contains("rate"))
            {
                decimal rate = Convert.ToDecimal(result["rate"]);

                // Armazenar taxa em cache por 1 hora
                _cache.Set(cacheKey, rate, TimeSpan.FromHours(1));

                return rate;
            }

            // Buscar da API externa se não estiver em cache ou banco de dados
            // Nota: Em um cenário real, você usaria uma API paga como Fixer.io, ExchangeRate-API, etc.
            // Para fins de exemplo, vamos usar uma API gratuita sem autenticação
            var url = $"https://api.exchangerate-api.com/v4/latest/{fromCurrency}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var exchangeData = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content);

                if (exchangeData?.Rates != null && exchangeData.Rates.ContainsKey(toCurrency))
                {
                    decimal rate = exchangeData.Rates[toCurrency];

                    // Salvar no banco de dados
                    await SaveExchangeRateAsync(fromCurrency, toCurrency, rate);

                    // Armazenar taxa em cache por 1 hora
                    _cache.Set(cacheKey, rate, TimeSpan.FromHours(1));

                    return rate;
                }

                throw new Exception($"Taxa não encontrada para {fromCurrency} para {toCurrency}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar taxa de câmbio: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<string>> GetAvailableCurrenciesAsync()
        {
            // Verificar cache primeiro
            string cacheKey = "AvailableCurrencies";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<string> cachedCurrencies))
            {
                return cachedCurrencies;
            }

            // Buscar da API
            var url = "https://api.exchangerate-api.com/v4/latest/EUR";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Para solução mais direta sem depender de uma classe modelada
                using (JsonDocument document = JsonDocument.Parse(content))
                {
                    if (document.RootElement.TryGetProperty("rates", out JsonElement ratesElement))
                    {
                        var currencies = new List<string>();
                        foreach (JsonProperty property in ratesElement.EnumerateObject())
                        {
                            currencies.Add(property.Name);
                        }

                        // Armazenar em cache por 24 horas
                        _cache.Set(cacheKey, currencies, TimeSpan.FromHours(24));
                        return currencies;
                    }
                }

                throw new Exception("Falha ao buscar moedas disponíveis - 'rates' não encontrado na resposta");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar moedas disponíveis: {ex.Message}", ex);
            }
        }

        public async Task UpdateExchangeRatesAsync()
        {
            var baseCurrencies = new[] { "EUR", "USD", "GBP" };
            var allCurrencies = await GetAvailableCurrenciesAsync();

            foreach (var baseCurrency in baseCurrencies)
            {
                var url = $"https://api.exchangerate-api.com/v4/latest/{baseCurrency}";

                try
                {
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var exchangeData = JsonSerializer.Deserialize<ExchangeRateApiResponse>(content);

                    if (exchangeData?.Rates != null)
                    {
                        foreach (var (currency, rate) in exchangeData.Rates)
                        {
                            await SaveExchangeRateAsync(baseCurrency, currency, rate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao atualizar taxas para {baseCurrency}: {ex.Message}");
                }
            }
        }

        private async Task SaveExchangeRateAsync(string fromCurrency, string toCurrency, decimal rate)
        {
            var query = @"
                INSERT INTO CurrencyConversions 
                (stamp_entity, from_currency, to_currency, rate, fetched_at, created_at) 
                VALUES (@stampEntity, @fromCurrency, @toCurrency, @rate, @fetchedAt, @createdAt)";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", Guid.NewGuid().ToString()),
                new SqlParameter("@fromCurrency", fromCurrency),
                new SqlParameter("@toCurrency", toCurrency),
                new SqlParameter("@rate", rate),
                new SqlParameter("@fetchedAt", DateTime.UtcNow),
                new SqlParameter("@createdAt", DateTime.UtcNow)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }
    }

    public class ExchangeRateApiResponse
    {
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }

    // Implementar um background service para atualizar taxas diariamente
    public class ExchangeRateUpdateService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExchangeRateUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Atualizar taxas de câmbio
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
                    await service.UpdateExchangeRatesAsync();
                }

                // Aguardar 24 horas
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
