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
    Task<Transaction?> GetByIdAsync(long id, CancellationToken ct);
    Task<Transaction?> GetByIdForUpdateAsync(long id, CancellationToken ct);
    Task<Transaction?> GetByIdWithCategoryAsync(long id, CancellationToken ct);
    Task<List<Transaction>?> GetInstallmentsForUpdateAsync(Guid installmentGroupId, CancellationToken ct);
    Task<InstallmentGroupHeader?> GetInstallmentGroupHeaderAsync(Guid installmentGroupId, CancellationToken ct);
    Task<PagedResult<Transaction>> GetInstallmentsByGroupIdAsync(Guid installmentGroupId, int pageNumber, int pageSize, CancellationToken ct);
    Task<PagedResult<Transaction>> ListByPeriodAsync(DateOnly startDate, DateOnly endDate, TransactionType? type, long? categoryId, int pageNumber, int pageSize, CancellationToken ct);
    Task<decimal> GetTotalForPeriodAsync(long? categoryId, DateOnly start, DateOnly end, CancellationToken ct, TransactionType type = TransactionType.Expense);
    Task<Dictionary<DateOnly, decimal>> GetTotalsByDateExcludingAsync(long? categoryId, DateOnly start, DateOnly end, IReadOnlyCollection<long> excludeTransactionIds, CancellationToken ct, TransactionType type = TransactionType.Expense);
    Task<bool> ExistsForCategoryAsync(long categoryId, CancellationToken ct);
}