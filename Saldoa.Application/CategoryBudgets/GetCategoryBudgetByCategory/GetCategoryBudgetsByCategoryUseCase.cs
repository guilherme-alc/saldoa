using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

public class GetCategoryBudgetsByCategoryUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;

    public GetCategoryBudgetsByCategoryUseCase(ICategoryBudgetRepository categoryBudgetRepository)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
    }

    public async Task<Result<PagedResult<CategoryBudgetResponse>>> ExecuteAsync(
        string userId, 
        long categoryId, 
        GetCategoryBudgetsByCategoryRequest request, 
        CancellationToken ct)
    {
        var data = await _categoryBudgetRepository.GetByCategoryAsync(
            userId, 
            categoryId, 
            request.PageNumber, 
            request.PageSize, 
            ct);

        var categoryBudgets = data.Items.Select(c => 
            new CategoryBudgetResponse(
                c.Id,
                c.CategoryId,
                c.PeriodStart,
                c.PeriodEnd,
                c.LimitAmount
            )
        ).ToList();

        var result = new PagedResult<CategoryBudgetResponse>(
            categoryBudgets,
            data.TotalCount,
            data.PageNumber,
            data.PageSize
        );
        
        return Result<PagedResult<CategoryBudgetResponse>>.Success(result);
    }
}