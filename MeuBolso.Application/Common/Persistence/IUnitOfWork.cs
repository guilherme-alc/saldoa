using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Transactions.Abstractions;

namespace MeuBolso.Application.Common.Persistence;

public interface IUnitOfWork
{
    ITransactionRepository Transactions { get; }
    ICategoryRepository Categories { get; }
    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}