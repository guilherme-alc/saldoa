using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Abstractions;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task RemoveAsync(Category category);
    Task<Category?> GetByIdAsync(long id);
    Task<PagedResult<Category>> ListAsync(int pageNumber, int pageSize);
}