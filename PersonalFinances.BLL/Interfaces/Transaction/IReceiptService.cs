using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PersonalFinances.BLL.Entities.Models.Transaction;

namespace PersonalFinances.BLL.Interfaces.Transaction
{
    public interface IReceiptService
    {
        Task<ReceiptModel> UploadReceiptAsync(string userId, IFormFile receiptImage);
        Task<ReceiptModel> GetReceiptByIdAsync(string receiptId);
        Task<IEnumerable<ReceiptModel>> GetUserReceiptsAsync(string userId);
        Task<ReceiptModel> ProcessReceiptAsync(string receiptId);
        Task LinkReceiptToTransactionAsync(string receiptId, string transactionId);
    }
}
