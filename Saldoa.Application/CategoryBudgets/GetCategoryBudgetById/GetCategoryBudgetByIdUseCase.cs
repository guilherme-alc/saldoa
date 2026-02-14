using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;

public class GetCategoryBudgetByIdUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;

    public GetCategoryBudgetByIdUseCase(ICategoryBudgetRepository categoryBudgetRepository)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
    }

    public async Task<Result<CategoryBudgetResponse>> ExecuteAsync(string userId, long categoryBudgetId, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdAsync(categoryBudgetId, userId, ct);
        
        if(categoryBudget is null)
            return Result<CategoryBudgetResponse>.Failure("Gasto por categoria não encontrado");
        
        var response = new CategoryBudgetResponse(
            categoryBudget.Id, 
            categoryBudget.CategoryId, 
            categoryBudget.PeriodStart, 
            categoryBudget.PeriodEnd, 
            categoryBudget.LimitAmount);
        
        return Result<CategoryBudgetResponse>.Success(response);
    } 
}