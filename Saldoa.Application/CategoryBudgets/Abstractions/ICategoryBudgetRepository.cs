using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.CategoryBudgets.Abstractions;

public interface ICategoryBudgetRepository
{
    Task AddAsync(CategoryBudget categoryBudget, CancellationToken ct);
    void Remove(CategoryBudget categoryBudget);
    Task<CategoryBudget?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<CategoryBudget?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<PagedResult<CategoryBudget>> ListAsync(int pageNumber, int pageSize, string userId, CancellationToken ct); 
    Task<bool> ExistsForPeriodAsync(string userId, long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct);
}