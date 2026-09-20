using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.CategoryBudgets.Abstractions;

public interface ICategoryBudgetRepository
{
    Task AddAsync(CategoryBudget categoryBudget, CancellationToken ct);
    void Remove(CategoryBudget categoryBudget);
    Task<CategoryBudget?> GetByIdAsync(long id, Guid workspaceId, CancellationToken ct);
    Task<CategoryBudget?> GetByIdForUpdateAsync(long id, Guid workspaceId, CancellationToken ct);
    Task<PagedResult<CategoryBudget>> ListAsync(int pageNumber, int pageSize, Guid workspaceId, DateOnly? startDate, DateOnly? endDate, bool? active, CancellationToken ct); 
    Task<bool> ExistsForPeriodAsync(Guid workspaceId, long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<bool> ExistsForPeriodAsync(Guid workspaceId, long categoryId, long categoryBudgetId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<List<CategoryBudget>> GetActiveForPeriodAsync(Guid workspaceId, long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
    Task<PagedResult<CategoryBudget>> GetByCategoryAsync(Guid workspaceId, long categoryId, int pageNumber, int pageSize, CancellationToken ct);
}