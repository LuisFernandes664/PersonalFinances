using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinances.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartDataController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public ChartDataController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData([FromQuery] string interval)
        {
            // Obter todas as transacções reais
            var transactions = await _transactionService.GetTransactionsAsync();

            List<ChartSeriesDto> seriesList = new List<ChartSeriesDto>();

            if (transactions == null || !transactions.Any())
            {
                // Se não houver transacções, retorna uma série vazia
                var emptySeries = new ChartSeriesDto
                {
                    Name = "Total Sales",
                    Data = new List<decimal>(),
                    Categories = new List<string>()
                };
                return Ok(APIResponse<IEnumerable<ChartSeriesDto>>.SuccessResponse(new List<ChartSeriesDto> { emptySeries }, "No data available."));
            }

            // Caso não seja especificado, usa "daily" como padrão
            interval = interval?.ToLower() ?? "daily";

            ChartSeriesDto series = new ChartSeriesDto
            {
                Name = "Total Sales",
                Data = new List<decimal>(),
                Categories = new List<string>()
            };

            switch (interval)
            {
                case "daily":
                    // Agrupa por data (dia)
                    var dailyGroups = transactions
                        .GroupBy(t => t.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g => new { Date = g.Key, Sum = g.Sum(t => t.Amount) })
                        .ToList();
                    foreach (var group in dailyGroups)
                    {
                        // Formata a data (ex.: "MM/dd")
                        series.Categories.Add(group.Date.ToString("MM/dd"));
                        series.Data.Add(group.Sum);
                    }
                    break;
                case "weekly":
                    // Usa o calendário atual para agrupar por número da semana
                    var culture = CultureInfo.CurrentCulture;
                    var weeklyGroups = transactions
                        .GroupBy(t => culture.Calendar.GetWeekOfYear(t.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                        .OrderBy(g => g.Key)
                        .Select(g => new { Week = g.Key, Sum = g.Sum(t => t.Amount) })
                        .ToList();
                    foreach (var group in weeklyGroups)
                    {
                        series.Categories.Add("Week " + group.Week);
                        series.Data.Add(group.Sum);
                    }
                    break;
                case "monthly":
                    // Agrupa por mês e ano
                    var monthlyGroups = transactions
                        .GroupBy(t => new { t.Date.Year, t.Date.Month })
                        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                        .Select(g => new { g.Key.Year, g.Key.Month, Sum = g.Sum(t => t.Amount) })
                        .ToList();
                    foreach (var group in monthlyGroups)
                    {
                        series.Categories.Add($"{group.Month}/{group.Year}");
                        series.Data.Add(group.Sum);
                    }
                    break;
                default:
                    // Valor default: daily
                    var defaultGroups = transactions
                        .GroupBy(t => t.Date.Date)
                        .OrderBy(g => g.Key)
                        .Select(g => new { Date = g.Key, Sum = g.Sum(t => t.Amount) })
                        .ToList();
                    foreach (var group in defaultGroups)
                    {
                        series.Categories.Add(group.Date.ToString("MM/dd"));
                        series.Data.Add(group.Sum);
                    }
                    break;
            }

            seriesList.Add(series);
            var response = APIResponse<IEnumerable<ChartSeriesDto>>.SuccessResponse(seriesList, "Chart data fetched successfully.");
            return Ok(response);
        }
    }
}
