using MeuBolso.Application.Common.Pagination;
using MeuBolso.Application.Transactions.Abstractions;
using MeuBolso.Domain.Entities;
using MeuBolso.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.Infrastructure.Transactions;

public class TransactionRepository : ITransactionRepository
{
    private readonly MeuBolsoDbContext _dbContext;
    public TransactionRepository(MeuBolsoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddAsync(Transaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
    }

    public void RemoveAsync(Transaction transaction)
    {
        _dbContext.Transactions.Remove(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(long id, string userId)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<Transaction?> GetByIdForUpdateAsync(long id, string userId)
    {
        return await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<PagedResult<Transaction>> ListByPeriodAsync(string userId, DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        var query = _dbContext
            .Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.PaidOrReceivedAt >= startDate &&
                        t.PaidOrReceivedAt <= endDate);
        
        var total = await query.CountAsync();

        var data = await query
            .OrderByDescending(t => t.PaidOrReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<Transaction>(data, total, pageNumber, pageSize);
    }
}