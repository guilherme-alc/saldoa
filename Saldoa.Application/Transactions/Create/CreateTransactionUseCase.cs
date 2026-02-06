using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Transactions.Create;

public class CreateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryBudgetRepository _categoryBudgetRepository;
    private readonly IUnitOfWork _unit;
    
    public CreateTransactionUseCase(ITransactionRepository transactionRepository, 
        IUnitOfWork unit, 
        ICategoryRepository categoryRepository,
        ICategoryBudgetRepository categoryBudgetRepository)
    {
        _transactionRepository = transactionRepository;
        _unit = unit;
        _categoryRepository = categoryRepository;
        _categoryBudgetRepository = categoryBudgetRepository;
    }

    public async Task<Result<TransactionResponse>> ExecuteAsync(CreateTransactionRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        
        if(category == null)
            return Result<TransactionResponse>.Failure("Categoria não encontrada");

        if (request.PaidOrReceivedAt.HasValue)
        {
            var budget = await _categoryBudgetRepository
                .GetActiveForPeriodAsync(
                    userId,
                    request.CategoryId,
                    request.PaidOrReceivedAt.Value,
                    ct);
            
            if (budget is not null)
            {
                var spent = await _transactionRepository
                    .GetTotalForPeriodAsync(
                        userId,
                        request.CategoryId,
                        budget.PeriodStart,
                        budget.PeriodEnd,
                        ct);

                var totalAfter = spent + request.Amount;

                if (totalAfter > budget.LimitAmount)
                    return Result<TransactionResponse>.Failure(
                        "Limite da categoria excedido para o período informado.");
            }
        }
        
        var transaction = new Transaction(
            userId, 
            request.Title, 
            request.Description, 
            request.Type, 
            request.Amount, 
            request.PaidOrReceivedAt, 
            category.Id);

        await _transactionRepository.AddAsync(transaction, ct);
        await _unit.SaveChangesAsync(ct);
        
        return Result<TransactionResponse>.Success(
            new TransactionResponse(
                transaction.Id, 
                transaction.Title, 
                transaction.Description, 
                transaction.Type, 
                transaction.Amount, 
                transaction.PaidOrReceivedAt, 
                new CategorySummaryResponse(category.Id, category.Name, category.Color))
        );
    }
}