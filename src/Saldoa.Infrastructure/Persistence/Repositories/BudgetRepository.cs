using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Common.Pagination;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Infrastructure.Persistence.Repositories;

public class BudgetRepository(SaldoaDbContext dbContext) : IBudgetRepository
{
    public async Task AddAsync(Budget budget, CancellationToken ct)
    {
        await dbContext.Budgets.AddAsync(budget, ct);
    }

    public void Remove(Budget budget)
    {
        dbContext.Budgets.Remove(budget);
    }

    public async Task<Budget?> GetByIdAsync(long id, CancellationToken ct)
    {
        var budget = await dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        return budget;
    }

    public async Task<Budget?> GetByIdForUpdateAsync(long id, CancellationToken ct)
    {
        var budget = await dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == id, ct);

        return budget;
    }

    public async Task<PagedResult<Budget>> ListAsync(
        int pageNumber, 
        int pageSize,   
        DateOnly? startDate,
        DateOnly? endDate,
        bool? active,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var query = dbContext
            .Budgets
            .AsNoTracking();

        if (startDate.HasValue)
        {
            query = query.Where(b => b.PeriodEnd >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(b => b.PeriodStart <= endDate.Value);
        }
        
        if (active.HasValue)
        {
            if (active.Value)
            {
                query = query.Where(b =>
                    b.PeriodStart <= today &&
                    b.PeriodEnd >= today);
            }
            else
            {
                query = query.Where(b =>
                    b.PeriodEnd < today ||
                    b.PeriodStart > today);
            }
        }
        
        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(b => b.PeriodStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Budget>(data, total, pageNumber, pageSize);
    }

    public async Task<bool> ExistsForPeriodAsync(    
        long? categoryId,
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken ct)
    {
        return await dbContext.Budgets.AnyAsync(
            b => b.CategoryId == categoryId
                 && b.PeriodStart <= periodEnd
                 && b.PeriodEnd >= periodStart,
            ct);
    }
    
    public async Task<bool> ExistsForPeriodAsync(  
        long? categoryId,
        long budgetId,
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken ct)
    {
        return await dbContext.Budgets.AnyAsync(
            b => b.Id != budgetId
                 && b.CategoryId == categoryId
                 && b.PeriodStart <= periodEnd
                 && b.PeriodEnd >= periodStart,
            ct);
    }

    public async Task<List<Budget>> GetActiveForPeriodAsync(long? categoryId, DateOnly periodStart, DateOnly periodEnd, CancellationToken ct)
    {
        return await dbContext.Budgets
            .AsNoTracking()
            .Where(b => (b.Scope == BudgetScope.General || b.CategoryId == categoryId)
                        && b.PeriodStart <= periodEnd
                        && b.PeriodEnd >= periodStart)
            .OrderBy(b => b.PeriodStart)
            .ThenBy(b => b.PeriodEnd)
            .ToListAsync(ct);
    }

    public async Task<PagedResult<Budget>> GetByCategoryAsync(long categoryId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = dbContext
            .Budgets
            .AsNoTracking()
            .Where(b => b.CategoryId == categoryId);
        
        var total = await query.CountAsync(ct);
        
        var data = await query
            .OrderBy(b => b.PeriodStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Budget>(data, total, pageNumber, pageSize);
    }
}