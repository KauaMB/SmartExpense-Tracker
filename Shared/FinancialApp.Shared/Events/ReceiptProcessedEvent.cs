namespace FinancialApp.Shared.Events;

public record ReceiptProcessedEvent(
    Guid TransactionId,         // Para o CoreService saber de qual transação é esse recibo
    decimal ExtractedTotal,     // O valor que a IA/OCR conseguiu ler
    string? MerchantName,       // O nome do mercado/loja (com '?' porque o OCR pode não conseguir ler)
    DateTime? ExtractedDate     // A data que estava no papel
);