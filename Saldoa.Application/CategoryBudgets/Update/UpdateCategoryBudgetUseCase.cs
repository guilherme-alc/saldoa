using Saldoa.Application.CategoryBudgets.Abstractions;
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

    public async Task<Result> ExecuteAsync(string userId, long categoryBudgetId, UpdateCategoryBudgetRequest request, CancellationToken ct)
    {
        var categoryBudget = await _categoryBudgetRepository.GetByIdForUpdateAsync(categoryBudgetId, userId, ct);
        
        if (categoryBudget == null)
            return Result.Failure("Limite de gasto por categoria não encontrado");
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if(categoryBudget.PeriodEnd < today)
            return Result.Failure("Não é possível alterar um limite de gasto já encerrado.");

        var newStart = request.PeriodStart ?? categoryBudget.PeriodStart;
        var newEnd = request.PeriodEnd ?? categoryBudget.PeriodEnd;

        if (newStart != categoryBudget.PeriodStart || newEnd != categoryBudget.PeriodEnd)
        {
            var exists = await _categoryBudgetRepository.ExistsForPeriodAsync(
                userId,
                categoryBudget.CategoryId,
                categoryBudget.Id,
                newStart,
                newEnd,
                ct);

            if (exists)
                return Result.Failure("Já existe um limite de gasto para essa categoria no período informado.");

            categoryBudget.SetPeriod(newStart, newEnd);
        }
        
        if(request.LimitAmount.HasValue && request.LimitAmount.Value > 0)
            categoryBudget.SetLimitAmount(request.LimitAmount.Value);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}