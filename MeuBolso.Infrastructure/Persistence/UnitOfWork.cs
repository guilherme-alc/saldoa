using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Transactions.Abstractions;
using MeuBolso.Infrastructure.Categories;
using MeuBolso.Infrastructure.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace MeuBolso.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    readonly MeuBolsoDbContext _dbContext;
    IDbContextTransaction? _transaction;
    
    ITransactionRepository? _transactionRepository;
    ICategoryRepository? _categoryRepository;

    public UnitOfWork(MeuBolsoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public ITransactionRepository Transactions => _transactionRepository ??= new TransactionRepository(_dbContext);
    public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(_dbContext);

    public async Task BeginTransactionAsync()
    {
        if(_transaction == null)
            _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _dbContext.SaveChangesAsync();
            
            await _transaction.CommitAsync();
            
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
    }
}