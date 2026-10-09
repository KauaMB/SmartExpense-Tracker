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
    // Substituímos os dois parâmetros soltos por um único objeto request
    public async Task<IActionResult> UploadReceipt([FromForm] UploadReceiptRequest request)
    {
        // Lembre-se de atualizar as chamadas para usar o request!
        if (request.File == null || request.File.Length == 0) return BadRequest(new { Error = "No image file was uploaded." });
        if (request.TransactionId == Guid.Empty) return BadRequest(new { Error = "Transaction ID is required." });

        try
        {
            // Passamos o request.File para o serviço da IA
            var extractedJson = await _ocrService.ProcessReceiptImageAsync(request.File);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ocrResult = JsonSerializer.Deserialize<OcrResponseTemp>(extractedJson, options);

            if (ocrResult == null) return StatusCode(500, "Falha ao interpretar o JSON da IA");

            var receiptEvent = new ReceiptProcessedEvent(
                request.TransactionId, // Aqui também usa o request
                ocrResult.TotalValue,
                ocrResult.MerchantName,
                DateTime.TryParse(ocrResult.Date, out var parsedDate) ? parsedDate : null,
                ocrResult.Items ?? new List<TransactionItemDto>()
            );

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

    public class UploadReceiptRequest
    {
        public IFormFile File { get; set; }
        public Guid TransactionId { get; set; }
    }
}