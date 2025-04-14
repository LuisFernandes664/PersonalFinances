using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using PersonalFinances.DAL.Helpers;

namespace PersonalFinances.DAL.Transaction
{
    public class ReceiptService : IReceiptService
    {
        private readonly DatabaseContext _dbContext;
        private readonly ITransactionService _transactionService;
        private readonly string _uploadFolder;

        public ReceiptService(
            DatabaseContext dbContext,
            ITransactionService transactionService,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
            _uploadFolder = configuration["ReceiptStorage:Path"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Receipts");

            // Garantir que a pasta de uploads existe
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        public async Task<ReceiptModel> UploadReceiptAsync(string userId, IFormFile receiptImage)
        {
            if (receiptImage == null || receiptImage.Length == 0)
                throw new ArgumentException("Arquivo não fornecido");

            // Gera nome e caminho do arquivo
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(receiptImage.FileName)}";
            var filePath = Path.Combine(_uploadFolder, fileName);

            // Garante que a pasta existe
            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);

            // Salva fisicamente
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await receiptImage.CopyToAsync(fileStream);
            }

            // Cria recibo
            var receipt = new ReceiptModel
            {
                StampEntity = Guid.NewGuid().ToString(),
                UserId = userId,
                MerchantName = "Pendente",
                TotalAmount = 0,
                ReceiptDate = DateTime.Now,
                IsProcessed = false,
                ProcessingStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImagePath = fileName
            };

            // Codifica imagem em Base64
            using (var memoryStream = new MemoryStream())
            {
                await receiptImage.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                receipt.ImageBase64 = Convert.ToBase64String(fileBytes);
                receipt.ContentType = receiptImage.ContentType;
            }

            var query = @"
                INSERT INTO Receipts (
                    stamp_entity, user_id, image_path, merchant_name, total_amount, receipt_date,
                    transaction_id, is_processed, processing_status, error_message,
                    image_base64, content_type, created_at, updated_at
                ) VALUES (
                    @stampEntity, @userId, @imagePath, @merchantName, @totalAmount, @receiptDate,
                    @transactionId, @isProcessed, @processingStatus, @errorMessage,
                    @imageBase64, @contentType, @createdAt, @updatedAt
                )";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@stampEntity", receipt.StampEntity),
                new SqlParameter("@userId", receipt.UserId),
                new SqlParameter("@imagePath", receipt.ImagePath),
                new SqlParameter("@merchantName", receipt.MerchantName ?? (object)DBNull.Value),
                new SqlParameter("@totalAmount", receipt.TotalAmount),
                new SqlParameter("@receiptDate", receipt.ReceiptDate),
                new SqlParameter("@transactionId", receipt.TransactionId ?? (object)DBNull.Value),
                new SqlParameter("@isProcessed", receipt.IsProcessed),
                new SqlParameter("@processingStatus", receipt.ProcessingStatus),
                new SqlParameter("@errorMessage", receipt.ErrorMessage ?? (object)DBNull.Value),
                new SqlParameter("@imageBase64", receipt.ImageBase64 ?? (object)DBNull.Value),
                new SqlParameter("@contentType", receipt.ContentType ?? (object)DBNull.Value),
                new SqlParameter("@createdAt", receipt.CreatedAt),
                new SqlParameter("@updatedAt", receipt.UpdatedAt)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);

            return receipt;
        }


        public async Task<ReceiptModel> GetReceiptByIdAsync(string receiptId)
        {
            var query = "SELECT * FROM Receipts WHERE stamp_entity = @receiptId";
            var parameters = new List<SqlParameter> { new SqlParameter("@receiptId", receiptId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            if (result.Rows.Count > 0)
            {
                return new ReceiptModel(result.Rows[0]);
            }

            return null;
        }

        public async Task<IEnumerable<ReceiptModel>> GetUserReceiptsAsync(string userId)
        {
            var query = "SELECT * FROM Receipts WHERE user_id = @userId ORDER BY created_at DESC";
            var parameters = new List<SqlParameter> { new SqlParameter("@userId", userId) };

            var result = await SQLHelper.ExecuteQueryAsync(query, parameters);
            var receipts = new List<ReceiptModel>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                receipts.Add(new ReceiptModel(row));
            }

            return receipts;
        }

        public async Task<ReceiptModel> ProcessReceiptAsync(string receiptId)
        {
            var receipt = await GetReceiptByIdAsync(receiptId);
            if (receipt == null)
            {
                throw new Exception("Recibo não encontrado.");
            }

            // Verificar se o recibo já foi processado
            if (receipt.IsProcessed)
            {
                return receipt;
            }

            try
            {
                // Obter caminho completo do arquivo
                var filePath = Path.Combine(_uploadFolder, receipt.ImagePath);

                // Implementação básica - em produção, aqui você usaria um serviço OCR real
                // Como o Google Cloud Vision API, Azure Form Recognizer, etc.

                // Valores de exemplo para demonstração
                receipt.MerchantName = "Supermercado Exemplo";
                receipt.TotalAmount = 42.50m;
                receipt.ReceiptDate = DateTime.Now.AddDays(-1);
                receipt.IsProcessed = true;
                receipt.ProcessingStatus = "Completed";

                // Atualizar no banco de dados
                var query = @"
                    UPDATE Receipts
                    SET merchant_name = @merchantName,
                        total_amount = @totalAmount,
                        receipt_date = @receiptDate,
                        is_processed = @isProcessed,
                        processing_status = @processingStatus,
                        error_message = @errorMessage,
                        updated_at = @updatedAt
                    WHERE stamp_entity = @receiptId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@receiptId", receipt.StampEntity),
                    new SqlParameter("@merchantName", receipt.MerchantName),
                    new SqlParameter("@totalAmount", receipt.TotalAmount),
                    new SqlParameter("@receiptDate", receipt.ReceiptDate),
                    new SqlParameter("@isProcessed", receipt.IsProcessed),
                    new SqlParameter("@processingStatus", receipt.ProcessingStatus),
                    new SqlParameter("@errorMessage", receipt.ErrorMessage ?? (object)DBNull.Value),
                    new SqlParameter("@updatedAt", DateTime.UtcNow)
                };

                await SQLHelper.ExecuteNonQueryAsync(query, parameters);

                return receipt;
            }
            catch (Exception ex)
            {
                // Atualizar o status de erro
                receipt.IsProcessed = false;
                receipt.ProcessingStatus = "Error";
                receipt.ErrorMessage = ex.Message;

                // Atualizar no banco de dados
                var query = @"
                    UPDATE Receipts
                    SET is_processed = @isProcessed,
                        processing_status = @processingStatus,
                        error_message = @errorMessage,
                        updated_at = @updatedAt
                    WHERE stamp_entity = @receiptId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@receiptId", receipt.StampEntity),
                    new SqlParameter("@isProcessed", receipt.IsProcessed),
                    new SqlParameter("@processingStatus", receipt.ProcessingStatus),
                    new SqlParameter("@errorMessage", receipt.ErrorMessage),
                    new SqlParameter("@updatedAt", DateTime.UtcNow)
                };

                await SQLHelper.ExecuteNonQueryAsync(query, parameters);

                throw;
            }
        }

        public async Task LinkReceiptToTransactionAsync(string receiptId, string transactionId)
        {
            var receipt = await GetReceiptByIdAsync(receiptId);
            if (receipt == null)
            {
                throw new Exception("Recibo não encontrado.");
            }

            var transaction = await _transactionService.GetTransactionByStampEntityAsync(transactionId);
            if (transaction == null)
            {
                throw new Exception("Transação não encontrada.");
            }

            receipt.TransactionId = transactionId;

            var query = @"
                UPDATE Receipts
                SET transaction_id = @transactionId,
                    updated_at = @updatedAt
                WHERE stamp_entity = @receiptId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@receiptId", receipt.StampEntity),
                new SqlParameter("@transactionId", receipt.TransactionId),
                new SqlParameter("@updatedAt", DateTime.UtcNow)
            };

            await SQLHelper.ExecuteNonQueryAsync(query, parameters);
        }
    }

}
