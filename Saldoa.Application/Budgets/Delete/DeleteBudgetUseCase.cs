using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Budgets.Delete;

public class DeleteBudgetUseCase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBudgetUseCase(IBudgetRepository budgetRepository, IUnitOfWork unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> ExecuteAsync(long budgetId, CancellationToken ct)
    {
        var budget = await _budgetRepository.GetByIdForUpdateAsync(budgetId, ct);
        
        if(budget is null)
        {
            var error = BudgetErrors.NotFound;
            return Result.Failure(error);
        }
        
        _budgetRepository.Remove(budget);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}