using CoreService.Domain.Entities;

namespace CoreService.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task AddAsync(Transaction expense);
    Task UpdateAsync(Transaction expense);
    Task DeleteAsync(Transaction expense);
}