using Microsoft.AspNetCore.Mvc;

namespace FinancialApp.OcrService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    [HttpPost]
    public IActionResult UploadReceipt([FromForm] IFormFile file, [FromForm] Guid transactionId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Error = "No image file was uploaded." });
        }

        if (transactionId == Guid.Empty)
        {
            return BadRequest(new { Error = "Transaction ID is required." });
        }

        // Lógica futura: Gravar a imagem temporariamente e enviar para o motor de OCR...

        return Accepted(new { Message = "File successfully received. Processing started.", TransactionId = transactionId });
    }
}