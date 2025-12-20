using MeuBolso.Application.Common.Pagination;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Transactions.Abstractions;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    void RemoveAsync(Transaction transaction);
    Task<Transaction?> GetByIdAsync(long id, string userId);
    Task<Transaction?> GetByIdForUpdateAsync(long id, string userId);
    Task<PagedResult<Transaction>> ListByPeriodAsync(string userId, DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
}