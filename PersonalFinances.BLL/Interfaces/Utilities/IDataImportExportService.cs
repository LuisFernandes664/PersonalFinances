using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Utilities
{
    public interface IDataImportExportService
    {
        Task<ImportSummary> ImportTransactionsFromCsvAsync(string userId, Stream fileStream);
        Task<byte[]> ExportTransactionsAsCsvAsync(string userId, DateTime? startDate, DateTime? endDate);
        Task<byte[]> ExportTransactionsAsExcelAsync(string userId, DateTime? startDate, DateTime? endDate);
        Task<byte[]> ExportFinancialReportAsync(string userId, string reportType, DateTime startDate, DateTime endDate);
    }

    public class ImportSummary
    {
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<object> ImportedTransactions { get; set; } = new List<object>();
    }

}
