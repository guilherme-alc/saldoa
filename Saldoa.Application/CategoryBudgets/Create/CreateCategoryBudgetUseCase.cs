using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.CategoryBudgets.Create;

public class CreateCategoryBudgetUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    private readonly IWorkspaceContext _workspaceContext;

    public CreateCategoryBudgetUseCase(
        ICategoryBudgetRepository categoryBudgetRepository, 
        IUnitOfWork unit, 
        IWorkspaceContext workspaceContext,
        ICategoryRepository categoryRepository)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _unit = unit;
        _workspaceContext = workspaceContext;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<Result<CategoryBudgetResponse>> ExecuteAsync(CreateCategoryBudgetRequest request, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, ct);

        if (category == null)
        {
            var error = CategoryErrors.NotFound;
            return Result<CategoryBudgetResponse>.Failure(error);
        }

        if (await _categoryBudgetRepository.ExistsForPeriodAsync(
                request.CategoryId, 
                request.PeriodStart,
                request.PeriodEnd, ct))
        {
            var error = CategoryBudgetErrors.AlreadyExists;
            return Result<CategoryBudgetResponse>.Failure(error);
        }

        var categoryBudget = new CategoryBudget(request.CategoryId, request.PeriodStart, request.PeriodEnd, request.LimitAmount, _workspaceContext.WorkspaceId!.Value);

        await _categoryBudgetRepository.AddAsync(categoryBudget, ct);
        await _unit.SaveChangesAsync(ct);

        return Result<CategoryBudgetResponse>.Success(new CategoryBudgetResponse(
            categoryBudget.Id,
            categoryBudget.CategoryId,
            categoryBudget.PeriodStart,
            categoryBudget.PeriodEnd,
            categoryBudget.LimitAmount));
    }
}