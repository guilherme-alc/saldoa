using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;

namespace Saldoa.Application.Transactions.ListPending;

public class ListPendingTransactionsUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    
    public ListPendingTransactionsUseCase(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PagedResult<TransactionResponse>>> ExecuteAsync(string userId, ListPendingTransactionsRequest request, CancellationToken ct)
    {
        var data = await _transactionRepository.ListPendingAsync(
            userId,
            request.Type,
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