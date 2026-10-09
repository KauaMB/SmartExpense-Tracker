using FinancialApp.OcrService.Services;
using FinancialApp.Shared.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FinancialApp.OcrService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly GeminiOcrService _ocrService;
    private readonly IPublishEndpoint _publishEndpoint;

    // Injetamos o MassTransit (IPublishEndpoint) para enviar a mensagem
    public ReceiptsController(GeminiOcrService ocrService, IPublishEndpoint publishEndpoint)
    {
        _ocrService = ocrService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> UploadReceipt([FromForm] IFormFile file, [FromForm] Guid transactionId)
    {
        if (file == null || file.Length == 0) return BadRequest(new { Error = "No image file was uploaded." });
        if (transactionId == Guid.Empty) return BadRequest(new { Error = "Transaction ID is required." });

        try
        {
            // 1. Recebe o JSON da IA
            var extractedJson = await _ocrService.ProcessReceiptImageAsync(file);

            // 2. Faz o parsing do JSON para uma estrutura temporária[cite: 8]
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ocrResult = JsonSerializer.Deserialize<OcrResponseTemp>(extractedJson, options);

            // 3. Monta o evento oficial da nossa arquitetura com a lista de itens[cite: 8]
            var receiptEvent = new ReceiptProcessedEvent(
                transactionId,
                ocrResult.TotalValue,
                ocrResult.MerchantName,
                DateTime.TryParse(ocrResult.Date, out var parsedDate) ? parsedDate : null,
                ocrResult.Items ?? new List<TransactionItemDto>()
            );

            // 4. Publica o evento no RabbitMQ[cite: 8]
            await _publishEndpoint.Publish(receiptEvent);

            return Ok(new
            {
                Message = "Receipt successfully processed and event published.",
                PublishedData = receiptEvent
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error processing the image.", Details = ex.Message });
        }
    }

    // Classe auxiliar privada apenas para ajudar a converter o JSON cru do Gemini
    private class OcrResponseTemp
    {
        public string MerchantName { get; set; }
        public decimal TotalValue { get; set; }
        public string Date { get; set; }
        public List<TransactionItemDto> Items { get; set; }
    }
}