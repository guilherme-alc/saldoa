using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Transactions.GetInstallmentsByGroupId;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct);
    void Delete(Transaction transaction);
    Task DeleteByInstallmentGroupId(Guid installmentGroupId, CancellationToken ct);
    Task<Transaction?> GetByIdAsync(long id, Guid workspaceId, CancellationToken ct);
    Task<Transaction?> GetByIdForUpdateAsync(long id, Guid workspaceId, CancellationToken ct);
    Task<Transaction?> GetByIdWithCategoryAsync(long id, Guid workspaceId, CancellationToken ct);
    Task<List<Transaction>?> GetInstallmentsForUpdateAsync(Guid installmentGroupId, Guid workspaceId, CancellationToken ct);
    Task<InstallmentGroupHeader?> GetInstallmentGroupHeaderAsync(Guid installmentGroupId, Guid workspaceId, CancellationToken ct);
    Task<PagedResult<Transaction>> GetInstallmentsByGroupIdAsync(Guid installmentGroupId, Guid workspaceId, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByPeriodAsync(Guid workspaceId, DateOnly startDate, DateOnly endDate, TransactionType? type, long? categoryId, int pageNumber, int pageSize, CancellationToken ct);
    Task<decimal> GetTotalForPeriodAsync(Guid workspaceId, long categoryId, DateOnly start, DateOnly end, CancellationToken ct, TransactionType type = TransactionType.Expense);
    Task<Dictionary<DateOnly, decimal>> GetTotalsByDateAsync(Guid workspaceId, long categoryId, DateOnly start, DateOnly end, CancellationToken ct, TransactionType type = TransactionType.Expense);
    Task<Dictionary<DateOnly, decimal>> GetTotalsByDateExcludingAsync(Guid workspaceId, long categoryId, DateOnly start, DateOnly end, IReadOnlyCollection<long> excludeTransactionIds, CancellationToken ct, TransactionType type = TransactionType.Expense);
    Task<bool> ExistsForCategoryAsync(long categoryId, Guid workspaceId, CancellationToken ct);
}