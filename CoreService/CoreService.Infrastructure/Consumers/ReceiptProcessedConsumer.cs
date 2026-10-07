using FinancialApp.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoreService.Infrastructure.Consumers;

// A interface IConsumer indica ao MassTransit qual é o evento exato que queremos escutar
public class ReceiptProcessedConsumer : IConsumer<ReceiptProcessedEvent>
{
    private readonly ILogger<ReceiptProcessedConsumer> _logger;

    public ReceiptProcessedConsumer(ILogger<ReceiptProcessedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ReceiptProcessedEvent> context)
    {
        var message = context.Message;

        // Em vez de usar o velho Console.WriteLine, usamos o ILogger, que é a norma no mercado
        _logger.LogInformation(
            "OCR Results Received! Transaction: {Id}, Value: {Value}, Store: {MerchantName}",
            message.TransactionId,
            message.ExtractedTotal,
            message.MerchantName ?? "Desconhecido"
        );

        return Task.CompletedTask;
    }
}