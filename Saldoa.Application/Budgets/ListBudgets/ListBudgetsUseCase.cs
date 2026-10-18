using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Budgets.ListBudgets;

public class ListBudgetsUseCase
{
    private readonly IBudgetRepository _budgetRepository;
    public ListBudgetsUseCase(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Result<PagedResult<BudgetResponse>>> ExecuteAsync(ListBudgetsRequest request,
        CancellationToken ct)
    {
        var data = await _budgetRepository.ListAsync(request.PageNumber,
            request.PageSize,
            request.StartDate,
            request.EndDate,
            request.Active,
            ct);

        var budgetsResponse = data.Items.Select(b =>
            new BudgetResponse(
                b.Id,
                b.CategoryId,
                b.Scope,
                b.PeriodStart,
                b.PeriodEnd,
                b.LimitAmount
            )
        ).ToList();

        var result = new PagedResult<BudgetResponse>(budgetsResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return Result<PagedResult<BudgetResponse>>.Success(result);
    }
}