using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Exceptions;

namespace Saldoa.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SaldoaDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(SaldoaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            _transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new PersistenceConflictException(
                "Conflito de concorrência ao persistir dados.", ex);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new PersistenceConflictException(
                "Já existe um registro com esses dados.", ex);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation
            })
        {
            throw new PersistenceConflictException(
                "Não foi possível concluir por vínculo com outro registro.", ex);
        }
        catch (DbUpdateException ex) when (
        ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ExclusionViolation
        })
        {
            throw new PersistenceConflictException(
                "Já existe um limite para essa categoria no período informado.", ex);
        }
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
            await SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            await transaction.RollbackAsync(ct);
        }
        finally
        {
            _transaction = null;
            await transaction.DisposeAsync();
        }
    }
}