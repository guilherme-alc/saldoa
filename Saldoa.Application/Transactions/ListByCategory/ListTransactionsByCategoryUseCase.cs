using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.ListByCategory;

public class ListTransactionsByCategoryUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    public ListTransactionsByCategoryUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PagedResult<TransactionResponse>>> ExecuteAsync(
        string userId,
        ListTransactionsByCategoryRequest request,
        CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var startDate = request.StartDate ?? new DateOnly(today.Year, today.Month, 1);
        var endDate = request.EndDate ?? startDate.AddMonths(1).AddDays(-1);
        
        var data = await _transactionRepository.ListByCategoryAsync(
            userId,
            startDate,
            endDate,
            request.CategoryId,
            request.PageNumber, 
            request.PageSize, 
            ct);

        var transactionsResponse = data.Items.Select(t =>
                new TransactionResponse(
                    t.Id, 
                    t.Title, 
                    t.Description, 
                    t.Type, 
                    t.Amount, 
                    t.PaidOrReceivedAt, 
                    new CategorySummaryResponse(t.Category.Id, t.Category.Name, t.Category.Color)
                )
            )
            .ToList();

        var result =
            new PagedResult<TransactionResponse>(transactionsResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return Result<PagedResult<TransactionResponse>>.Success(result);
    }
}