using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Interfaces.Utilities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class DataExportController : ControllerBase
    {
        private readonly IDataImportExportService _service;

        public DataExportController(IDataImportExportService service)
        {
            _service = service;
        }

        [HttpPost("import/csv")]
        public async Task<IActionResult> ImportFromCsv(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            if (file == null || file.Length == 0)
                return BadRequest(APIResponse<object>.FailResponse("Nenhum arquivo foi enviado."));

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var summary = await _service.ImportTransactionsFromCsvAsync(userId, stream);
                    return Ok(APIResponse<object>.SuccessResponse(summary, "Importação concluída com sucesso."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao importar transações: {ex.Message}"));
            }
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportToCsv([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            try
            {
                var csvData = await _service.ExportTransactionsAsCsvAsync(userId, startDate, endDate);
                return File(csvData, "text/csv", $"transactions_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao exportar transações: {ex.Message}"));
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            try
            {
                var excelData = await _service.ExportTransactionsAsExcelAsync(userId, startDate, endDate);
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"transactions_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao exportar transações: {ex.Message}"));
            }
        }

        [HttpGet("report/{reportType}")]
        public async Task<IActionResult> GenerateReport(
            string reportType,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            try
            {
                var reportData = await _service.ExportFinancialReportAsync(userId, reportType, startDate, endDate);
                return File(reportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportType}_report_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao gerar relatório: {ex.Message}"));
            }
        }
    }
}