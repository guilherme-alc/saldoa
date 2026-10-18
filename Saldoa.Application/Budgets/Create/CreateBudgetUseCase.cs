using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Budgets.Abstractions;
using Saldoa.Application.Budgets.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Budgets.Create;

public class CreateBudgetUseCase
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    private readonly IWorkspaceContext _workspaceContext;

    public CreateBudgetUseCase(
        IBudgetRepository budgetRepository, 
        IUnitOfWork unit, 
        IWorkspaceContext workspaceContext,
        ICategoryRepository categoryRepository)
    {
        _budgetRepository = budgetRepository;
        _unit = unit;
        _workspaceContext = workspaceContext;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<Result<BudgetResponse>> ExecuteAsync(CreateBudgetRequest request, CancellationToken ct)
    {
        if(request.CategoryId != null)
        {
            if(request.Scope == BudgetScope.General)
            {
                var error = BudgetErrors.Invalid;
                return Result<BudgetResponse>.Failure(error);
            }
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, ct);
            if (request.Scope == BudgetScope.Category && category == null)
            {
                var error = CategoryErrors.NotFound;
                return Result<BudgetResponse>.Failure(error);
            }
        }

        if (await _budgetRepository.ExistsForPeriodAsync(
                request.CategoryId, 
                request.PeriodStart,
                request.PeriodEnd, ct))
        {
            var error = BudgetErrors.AlreadyExists;
            return Result<BudgetResponse>.Failure(error);
        }

        var budget = new Budget(request.CategoryId, request.Scope, request.PeriodStart, request.PeriodEnd, request.LimitAmount, _workspaceContext.WorkspaceId!.Value);

        await _budgetRepository.AddAsync(budget, ct);
        await _unit.SaveChangesAsync(ct);

        return Result<BudgetResponse>.Success(new BudgetResponse(
            budget.Id,
            budget.CategoryId,
            budget.Scope,
            budget.PeriodStart,
            budget.PeriodEnd,
            budget.LimitAmount));
    }
}