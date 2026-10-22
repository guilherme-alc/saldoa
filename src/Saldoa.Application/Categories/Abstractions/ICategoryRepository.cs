using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Categories.Abstractions;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken ct);
    void Remove(Category category);
    Task<Category?> GetByIdAsync(long id, CancellationToken ct);
    Task<Category?> GetByIdForUpdateAsync(long id, CancellationToken ct);
    Task<PagedResult<Category>> ListAsync(int pageNumber, int pageSize, CancellationToken ct); 
    Task<bool> ExistsAsync(string name, CancellationToken ct);
}