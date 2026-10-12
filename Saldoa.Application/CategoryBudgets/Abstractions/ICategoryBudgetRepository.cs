using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.CategoryBudgets.Abstractions;

public interface ICategoryBudgetRepository
{
    Task AddAsync(CategoryBudget categoryBudget, CancellationToken ct);
    void Remove(CategoryBudget categoryBudget);
    Task<CategoryBudget?> GetByIdAsync(long id, CancellationToken ct);
    Task<CategoryBudget?> GetByIdForUpdateAsync(long id,CancellationToken ct);
    Task<PagedResult<CategoryBudget>> ListAsync(int pageNumber, int pageSize, DateOnly? startDate, DateOnly? endDate, bool? active, CancellationToken ct); 
    Task<bool> ExistsForPeriodAsync(long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<bool> ExistsForPeriodAsync(long categoryId, long categoryBudgetId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<List<CategoryBudget>> GetActiveForPeriodAsync(long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<PagedResult<CategoryBudget>> GetByCategoryAsync(long categoryId, int pageNumber, int pageSize, CancellationToken ct);
}