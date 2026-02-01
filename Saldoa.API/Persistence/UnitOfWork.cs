using Saldoa.Application.Common.Abstractions;

namespace Saldoa.API.Persistence;

public class UnitOfWork : IUnitOfWork
{
    readonly SaldoaDbContext _dbContext;

    public UnitOfWork(SaldoaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveChangesAsync(
        CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}