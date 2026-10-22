using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Budgets.GetBudgetByCategory;

public class GetBudgetsByCategoryUseCase
{
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetsByCategoryUseCase(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Result<PagedResult<BudgetResponse>>> ExecuteAsync(
        long categoryId, 
        GetBudgetsByCategoryRequest request, 
        CancellationToken ct)
    {
        var data = await _budgetRepository.GetByCategoryAsync(
            categoryId, 
            request.PageNumber, 
            request.PageSize, 
            ct);

        var categoryBudgets = data.Items.Select(c => 
            new BudgetResponse(
                c.Id,
                c.CategoryId,
                c.Scope,
                c.PeriodStart,
                c.PeriodEnd,
                c.LimitAmount
            )
        ).ToList();

        var result = new PagedResult<BudgetResponse>(
            categoryBudgets,
            data.TotalCount,
            data.PageNumber,
            data.PageSize
        );
        
        return Result<PagedResult<BudgetResponse>>.Success(result);
    }
}