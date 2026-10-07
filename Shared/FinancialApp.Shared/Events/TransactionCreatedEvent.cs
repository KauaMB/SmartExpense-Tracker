namespace FinancialApp.Shared.Events;

// Usamos 'record' porque eventos de rede são imutáveis (aconteceram no passado e não mudam)
public record ExpenseCreatedEvent(
    Guid ExpenseId,
    decimal TotalValue,
    string Category,
    DateTime OccurredOn
);