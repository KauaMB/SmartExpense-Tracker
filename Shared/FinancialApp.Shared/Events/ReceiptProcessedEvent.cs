namespace FinancialApp.Shared.Events;

public record ReceiptProcessedEvent(
    Guid TransactionId,
    decimal ExtractedTotal,
    string MerchantName,
    DateTime? ExtractedDate,
    List<TransactionItemDto> Items
);