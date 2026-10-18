using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Budgets.Update;

public class UpdateBudgetUseCase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unit;

    public UpdateBudgetUseCase(IBudgetRepository budgetRepository, IUnitOfWork unit)
    {
        _budgetRepository = budgetRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long budgetId, UpdateBudgetRequest request, CancellationToken ct)
    {
        var budget = await _budgetRepository.GetByIdForUpdateAsync(budgetId, ct);
        
        if (budget == null)
        {
            var error = BudgetErrors.NotFound;
            return Result.Failure(error);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if(budget.PeriodEnd < today)
        {
            var error = BudgetErrors.ClosedPeriod;
            return Result.Failure(error);
        }

        if (request.PeriodStart != budget.PeriodStart || request.PeriodEnd != budget.PeriodEnd)
        {
            var exists = await _budgetRepository.ExistsForPeriodAsync(
                budget.CategoryId,
                budget.Id,
                request.PeriodStart,
                request.PeriodEnd,
                ct);

            if (exists)
            {
                var error = BudgetErrors.AlreadyExists;
                return Result.Failure(error);
            }

            budget.ChangePeriod(request.PeriodStart, request.PeriodEnd);
        }
        
        budget.ChangeLimit(request.LimitAmount);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}