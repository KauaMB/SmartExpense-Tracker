namespace CoreService.Domain.Entities;

public class TransactionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Chave Estrangeira (Foreign Key)
    public Guid TransactionId { get; set; }

    // Propriedade de navegação do Entity Framework
    public Transaction? Transaction { get; set; }
}