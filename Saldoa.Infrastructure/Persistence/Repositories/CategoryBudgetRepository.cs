using Microsoft.EntityFrameworkCore;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Repositories;

public class CategoryBudgetRepository(SaldoaDbContext dbContext) : ICategoryBudgetRepository
{
    public async Task AddAsync(CategoryBudget categoryBudget, CancellationToken ct)
    {
        await dbContext.CategoryBudgets.AddAsync(categoryBudget, ct);
    }

    public void Remove(CategoryBudget categoryBudget)
    {
        dbContext.CategoryBudgets.Remove(categoryBudget);
    }

    public async Task<CategoryBudget?> GetByIdAsync(long id, Guid workspaceId, CancellationToken ct)
    {
        var categoryBudget = await dbContext.CategoryBudgets
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId, ct);

        return categoryBudget;
    }

    public async Task<CategoryBudget?> GetByIdForUpdateAsync(long id, Guid workspaceId, CancellationToken ct)
    {
        var categoryBudget = await dbContext.CategoryBudgets
            .FirstOrDefaultAsync(c => c.Id == id && c.WorkspaceId == workspaceId, ct);

        return categoryBudget;
    }

    public async Task<PagedResult<CategoryBudget>> ListAsync(
        int pageNumber, 
        int pageSize, 
        Guid workspaceId,
        DateOnly? startDate,
        DateOnly? endDate,
        bool? active,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var query = dbContext
            .CategoryBudgets
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId);

        if (startDate.HasValue)
        {
            query = query.Where(c => c.PeriodEnd >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(c => c.PeriodStart <= endDate.Value);
        }
        
        if (active.HasValue)
        {
            if (active.Value)
            {
                query = query.Where(c =>
                    c.PeriodStart <= today &&
                    c.PeriodEnd >= today);
            }
            else
            {
                query = query.Where(c =>
                    c.PeriodEnd < today ||
                    c.PeriodStart > today);
            }
        }
        
        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(c => c.PeriodStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<CategoryBudget>(data, total, pageNumber, pageSize);
    }

    public async Task<bool> ExistsForPeriodAsync(
        Guid workspaceId,
        long categoryId,
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken ct)
    {
        return await dbContext.CategoryBudgets.AnyAsync(
            c => c.WorkspaceId == workspaceId 
                 && c.CategoryId == categoryId
                 && c.PeriodStart <= periodEnd
                 && c.PeriodEnd >= periodStart,
            ct);
    }
    
    public async Task<bool> ExistsForPeriodAsync(
        Guid workspaceId,
        long categoryId,
        long categoryBudgetId,
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken ct)
    {
        return await dbContext.CategoryBudgets.AnyAsync(
            c => c.WorkspaceId == workspaceId 
                 && c.Id != categoryBudgetId
                 && c.CategoryId == categoryId
                 && c.PeriodStart <= periodEnd
                 && c.PeriodEnd >= periodStart,
            ct);
    }

    public async Task<List<CategoryBudget>> GetActiveForPeriodAsync(Guid workspaceId, long categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct)
    {
        return await dbContext.CategoryBudgets
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId
                        && c.CategoryId == categoryId
                        && c.PeriodStart <= periodEnd
                        && c.PeriodEnd >= periodStart)
            .OrderBy(b => b.PeriodStart)
            .ThenBy(b => b.PeriodEnd)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<CategoryBudget>> GetByCategoryAsync(Guid workspaceId, long categoryId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = dbContext
            .CategoryBudgets
            .AsNoTracking()
            .Where(c => c.WorkspaceId == workspaceId && c.CategoryId == categoryId);
        
        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(c => c.PeriodStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<CategoryBudget>(data, total, pageNumber, pageSize);
    }
}