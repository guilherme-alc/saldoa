using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.CategoryBudgets.Update;

public class UpdateCategoryBudgetUseCase
{
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly IUnitOfWork _unit;

    public UpdateCategoryBudgetUseCase(ICategoryBudgetRepository categoryBudgetRepository, IUnitOfWork unit)
    {
        _categoryBudgetRepository = categoryBudgetRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(Guid workspaceId, long categoryBudgetId, UpdateCategoryBudgetRequest request, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdForUpdateAsync(categoryBudgetId, workspaceId, ct);
        
        if (categoryBudget == null)
        {
            var error = CategoryBudgetErrors.NotFound;
            return Result.Failure(error);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if(categoryBudget.PeriodEnd < today)
        {
            var error = CategoryBudgetErrors.ClosedPeriod;
            return Result.Failure(error);
        }

        if (request.PeriodStart != categoryBudget.PeriodStart || request.PeriodEnd != categoryBudget.PeriodEnd)
        {
            var exists = await _categoryBudgetRepository.ExistsForPeriodAsync(
                workspaceId,
                categoryBudget.CategoryId,
                categoryBudget.Id,
                request.PeriodStart,
                request.PeriodEnd,
                ct);

            if (exists)
            {
                var error = CategoryBudgetErrors.AlreadyExists;
                return Result.Failure(error);
            }

            categoryBudget.ChangePeriod(request.PeriodStart, request.PeriodEnd);
        }
        
        categoryBudget.ChangeLimit(request.LimitAmount);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}