using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Transactions.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct);
    void Remove(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<Transaction?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByPeriodAsync(string userId, DateOnly startDate, DateOnly endDate, int pageNumber, int pageSize, CancellationToken ct);
}