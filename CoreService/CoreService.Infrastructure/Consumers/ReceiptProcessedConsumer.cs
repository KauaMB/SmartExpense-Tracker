using CoreService.Domain.Entities;
using CoreService.Domain.Interfaces;
using FinancialApp.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CoreService.Infrastructure.Consumers;

public class ReceiptProcessedConsumer : IConsumer<ReceiptProcessedEvent>
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<ReceiptProcessedConsumer> _logger;

    public ReceiptProcessedConsumer(ITransactionRepository repository, ILogger<ReceiptProcessedConsumer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReceiptProcessedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("A processar recibo para a Transação {TransactionId}...", message.TransactionId);

        // 1. Buscar a transação original na base de dados
        var transaction = await _repository.GetByIdAsync(message.TransactionId);

        if (transaction == null)
        {
            _logger.LogWarning("Transação {TransactionId} não foi encontrada. Evento ignorado.", message.TransactionId);
            return;
        }

        // 2. Atualizar os dados de cabeçalho da fatura
        transaction.TotalValue = message.ExtractedTotal;
        transaction.MerchantName = message.MerchantName;

        if (message.ExtractedDate.HasValue)
        {
            transaction.OccurredOn = message.ExtractedDate.Value;
        }

        // 3. Iterar sobre a lista de itens recebida (O foreach que a task pede)
        if (message.Items != null && message.Items.Any())
        {
            foreach (var itemDto in message.Items)
            {
                var transactionItem = new TransactionItem
                {
                    Name = itemDto.Name,
                    Category = itemDto.Category,
                    Price = itemDto.Price,
                    TransactionId = transaction.Id // Vinculamos a chave estrangeira (Foreign Key)
                };

                // Adiciona o novo produto à lista de itens desta transação específica
                transaction.Items.Add(transactionItem);
            }
        }

        // 4. Salvar as alterações e os novos itens na base de dados
        await _repository.UpdateAsync(transaction);

        _logger.LogInformation(
            "Transação {TransactionId} atualizada com sucesso! {ItemCount} produtos categorizados e guardados.",
            message.TransactionId,
            message.Items?.Count ?? 0
        );
    }
}