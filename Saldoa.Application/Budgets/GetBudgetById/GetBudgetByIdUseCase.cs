using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;

namespace Saldoa.Application.Budgets.GetBudgetById;

public class GetBudgetByIdUseCase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetBudgetByIdUseCase(IBudgetRepository budgetRepository, ITransactionRepository transactionRepository)
    {
        _budgetRepository = budgetRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<BudgetDetailsResponse>> ExecuteAsync(long budgetId, CancellationToken ct)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId, ct);
        
        if(budget is null)
        {
            var error = BudgetErrors.NotFound;
            return Result<BudgetDetailsResponse>.Failure(error);
        }

        var total = await _transactionRepository.GetTotalForPeriodAsync(
            budget.CategoryId, 
            budget.PeriodStart, 
            budget.PeriodEnd, 
            ct);
        
        var totalSpent = total;
        
        var remainingAmount = budget.LimitAmount - totalSpent;
        
        var percentageUsed = budget.LimitAmount == 0
            ? 0
            : (totalSpent / budget.LimitAmount) * 100;
        
        var response = new BudgetDetailsResponse(
            budget.Id, 
            budget.CategoryId, 
            budget.Scope,
            budget.PeriodStart, 
            budget.PeriodEnd, 
            budget.LimitAmount,
            totalSpent,
            remainingAmount,
            percentageUsed);
        
        return Result<BudgetDetailsResponse>.Success(response);
    } 
}