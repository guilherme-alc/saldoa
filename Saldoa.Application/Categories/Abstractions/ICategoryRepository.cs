using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Categories.Abstractions;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken ct);
    void Remove(Category category);
    Task<Category?> GetByIdAsync(long id, string userId, CancellationToken ct);
    Task<Category?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct);
    Task<PagedResult<Category>> ListAsync(int pageNumber, int pageSize, string userId, CancellationToken ct); 
    Task<bool> ExistsAsync(string userId, string name, CancellationToken ct);
}