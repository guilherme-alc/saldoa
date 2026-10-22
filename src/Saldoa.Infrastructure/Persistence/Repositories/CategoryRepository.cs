using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Repositories;

public class CategoryRepository(SaldoaDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken ct)
    {
        await dbContext.Categories.AddAsync(category, ct);
    }

    public void Remove(Category category)
    {
        dbContext.Categories.Remove(category);
    }

    public async Task<Category?> GetByIdAsync(long id, CancellationToken ct)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return category;
    }
    
    public async Task<Category?> GetByIdForUpdateAsync(long id, CancellationToken ct)
    {
        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return category;
    }

    public async Task<PagedResult<Category>> ListAsync(
        int pageNumber, 
        int pageSize,
        CancellationToken ct)
    {
        var query = dbContext
            .Categories
            .AsNoTracking();

        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Category>(data, total, pageNumber, pageSize);
    }

    public Task<bool> ExistsAsync(string name, CancellationToken ct)
    {
        var nameNormalized = name.Trim().ToUpperInvariant();
        
        return dbContext.Categories.AnyAsync(
            c => c.NormalizedName == nameNormalized,
            ct);
    }
}