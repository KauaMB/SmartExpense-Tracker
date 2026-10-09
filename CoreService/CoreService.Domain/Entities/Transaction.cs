using System;

namespace CoreService.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? MerchantName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();
    }
}
