using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.BLL.Entities;
using PersonalFinances.BLL.Entities.Models.Transaction;
using PersonalFinances.BLL.Interfaces.Transaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinances.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Bearer")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadReceipt(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            if (file == null || file.Length == 0)
                return BadRequest(APIResponse<object>.FailResponse("Nenhum arquivo foi enviado."));

            // Validar se o arquivo é uma imagem
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return BadRequest(APIResponse<object>.FailResponse("Apenas imagens JPG e PNG são permitidas."));

            try
            {
                var receipt = await _receiptService.UploadReceiptAsync(userId, file);
                return Ok(APIResponse<ReceiptModel>.SuccessResponse(receipt, "Recibo enviado com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao processar o recibo: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserReceipts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(APIResponse<object>.FailResponse("Utilizador não autenticado."));

            var receipts = await _receiptService.GetUserReceiptsAsync(userId);
            return Ok(APIResponse<IEnumerable<ReceiptModel>>.SuccessResponse(receipts, "Recibos obtidos com sucesso."));
        }

        [HttpGet("{receiptId}")]
        public async Task<IActionResult> GetReceipt(string receiptId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var receipt = await _receiptService.GetReceiptByIdAsync(receiptId);

            if (receipt == null)
                return NotFound(APIResponse<object>.FailResponse("Recibo não encontrado."));

            if (receipt.UserId != userId)
                return Forbid();

            return Ok(APIResponse<ReceiptModel>.SuccessResponse(receipt, "Recibo obtido com sucesso."));
        }

        [HttpPost("{receiptId}/process")]
        public async Task<IActionResult> ProcessReceipt(string receiptId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var receipt = await _receiptService.GetReceiptByIdAsync(receiptId);

            if (receipt == null)
                return NotFound(APIResponse<object>.FailResponse("Recibo não encontrado."));

            if (receipt.UserId != userId)
                return Forbid();

            try
            {
                var processedReceipt = await _receiptService.ProcessReceiptAsync(receiptId);
                return Ok(APIResponse<ReceiptModel>.SuccessResponse(processedReceipt, "Recibo processado com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao processar o recibo: {ex.Message}"));
            }
        }

        [HttpPost("{receiptId}/link/{transactionId}")]
        public async Task<IActionResult> LinkToTransaction(string receiptId, string transactionId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var receipt = await _receiptService.GetReceiptByIdAsync(receiptId);

            if (receipt == null)
                return NotFound(APIResponse<object>.FailResponse("Recibo não encontrado."));

            if (receipt.UserId != userId)
                return Forbid();

            try
            {
                await _receiptService.LinkReceiptToTransactionAsync(receiptId, transactionId);
                return Ok(APIResponse<object>.SuccessResponse(null, "Recibo vinculado à transação com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.FailResponse($"Erro ao vincular recibo: {ex.Message}"));
            }
        }
    }
}