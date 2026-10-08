using CoreService.Domain.Repositories;
using FinancialApp.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoreService.Infrastructure.Consumers;

public class ReceiptProcessedConsumer : IConsumer<ReceiptProcessedEvent>
{
    private readonly ILogger<ReceiptProcessedConsumer> _logger;
    private readonly ITransactionRepository _transactionRepository;

    public ReceiptProcessedConsumer(
        ILogger<ReceiptProcessedConsumer> logger,
        ITransactionRepository transactionRepository)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
    }

    public async Task Consume(ConsumeContext<ReceiptProcessedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Processing OCR return for Transaction: {Id}", message.TransactionId);

        var transaction = await _transactionRepository.GetByIdAsync(message.TransactionId);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {Id} not found in the database. Ignoring event.", message.TransactionId);
            return;
        }

        transaction.TotalValue = message.ExtractedTotal;
        transaction.Establishment = message.MerchantName;

        if (message.ExtractedDate.HasValue)
        {
            transaction.CreatedAt = message.ExtractedDate.Value;
        }

        await _transactionRepository.UpdateAsync(transaction);

        _logger.LogInformation("Transaction {Id} successfully updated in the database!", message.TransactionId);
    }
}