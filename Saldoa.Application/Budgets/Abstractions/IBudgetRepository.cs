using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Budgets.Abstractions;

public interface IBudgetRepository
{
    Task AddAsync(Budget budget, CancellationToken ct);
    void Remove(Budget budget);
    Task<Budget?> GetByIdAsync(long id, CancellationToken ct);
    Task<Budget?> GetByIdForUpdateAsync(long id,CancellationToken ct);
    Task<PagedResult<Budget>> ListAsync(int pageNumber, int pageSize, DateOnly? startDate, DateOnly? endDate, bool? active, CancellationToken ct); 
    Task<bool> ExistsForPeriodAsync(long? categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<bool> ExistsForPeriodAsync(long? categoryId, long budgetId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<List<Budget>> GetActiveForPeriodAsync(long? categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<PagedResult<Budget>> GetByCategoryAsync(long categoryId, int pageNumber, int pageSize, CancellationToken ct);
}