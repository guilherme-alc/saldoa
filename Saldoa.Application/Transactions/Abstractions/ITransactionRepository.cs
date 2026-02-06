using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct);
    void Remove(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<Transaction?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<Transaction?> GetByIdWithCategoryAsync(long id, string userId, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByPeriodAsync(string userId, DateOnly startDate, DateOnly endDate, ETransactionType? type, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListPendingAsync(string userId, ETransactionType? type, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByCategoryAsync(string userId, DateOnly startDate, DateOnly endDate, long categoryId, int pageNumber, int pageSize, CancellationToken ct);
    Task<decimal> GetTotalForPeriodAsync(string userId, long categoryId, DateOnly start, DateOnly end, CancellationToken ct);
    Task<decimal> GetTotalForPeriodExcludingAsync(string userId, long categoryId, DateOnly start, DateOnly end, long excludeTransactionId, CancellationToken ct);
}