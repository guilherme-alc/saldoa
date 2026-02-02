using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Saldoa.Domain.Enums;

namespace Saldoa.API.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly SaldoaDbContext _dbContext;
    public TransactionRepository(SaldoaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _dbContext.Transactions.AddAsync(transaction, ct);
    }

    public void Remove(Transaction transaction)
    {
        _dbContext.Transactions.Remove(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(long id, string userId, CancellationToken ct = default)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    public async Task<Transaction?> GetByIdForUpdateAsync(long id, string userId, CancellationToken ct = default)
    {
        return await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }

    public async Task<Transaction?> GetByIdWithCategoryAsync(long id, string userId, CancellationToken ct)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
    }


    public async Task<PagedResult<Transaction>> ListByPeriodAsync(
        string userId, 
        DateOnly startDate, 
        DateOnly endDate, 
        ETransactionType? type,
        int pageNumber, 
        int pageSize, 
        CancellationToken ct = default)
    {
        var query = _dbContext
            .Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.PaidOrReceivedAt.HasValue &&
                        t.PaidOrReceivedAt.Value >= startDate &&
                        t.PaidOrReceivedAt.Value <= endDate &&
                        (type == null || t.Type == type));
        
        var total = await query.CountAsync(ct);

        var data = await query
            .Include(t => t.Category)
            .OrderByDescending(t => t.PaidOrReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PagedResult<Transaction>(data, total, pageNumber, pageSize);
    }
}