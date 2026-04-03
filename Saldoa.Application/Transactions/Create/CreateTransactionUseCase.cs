using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;
using Saldoa.Domain.ValueObjects;

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

    public async Task<Result<TransactionsResponse>> ExecuteAsync(CreateTransactionRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        
        if(category == null)
            return Result<TransactionsResponse>.Failure("Categoria não encontrada");

        var totalInstallment = request.TotalInstallments.HasValue && request.TotalInstallments.Value > 0
            ? request.TotalInstallments.Value
            : 1;

        if (request.PaidOrReceivedAt.HasValue)
        {
            var (baseValue, remainder) = CalculateInstallments(request.TotalAmount, totalInstallment);

            for (int i = 1; i <= totalInstallment; i++)
            {
                var amount = i == totalInstallment ? baseValue + remainder : baseValue;
                var date = request.PaidOrReceivedAt.Value.AddMonths(i - 1);

                var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                    userId,
                    request.CategoryId,
                    date,
                    ct);

                if (budget is not null)
                {
                    var spent = await _transactionRepository.GetTotalForPeriodAsync(
                        userId,
                        request.CategoryId,
                        budget.PeriodStart,
                        budget.PeriodEnd,
                        ct,
                        ETransactionType.Expense);

                    if (spent + amount > budget.LimitAmount)
                        return Result<TransactionsResponse>.Failure(
                            $"Limite excedido para {date:MM/yyyy}");
                }
            }
        }

        if (totalInstallment > 1)
        {
            var (baseValue, remainder) = CalculateInstallments(request.TotalAmount, totalInstallment);

            var groupId = Guid.CreateVersion7();

            var transactions = new List<Transaction>();

            for (int i = 1; i <= totalInstallment; i++)
            {
                var amount = i == totalInstallment ? baseValue + remainder : baseValue;

                var transaction = new Transaction(
                    userId,
                    request.Title,
                    request.Description,
                    request.Type,
                    amount,
                    request.PaidOrReceivedAt?.AddMonths(i - 1),
                    category.Id,
                    InstallmentInfo.Create(totalInstallment, i, groupId)
                );
                transactions.Add(transaction);
            }

            await _transactionRepository.AddRangeAsync(transactions, ct);
            await _unit.SaveChangesAsync(ct);

            var response = transactions.Select(t => 
                new TransactionResponse(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Type,
                    t.Amount,
                    t.PaidOrReceivedAt,
                    new CategorySummaryResponse(category.Id, category.Name, category.Color),
                    t.InstallmentInfo
                )
            )
            .ToList();

            return Result<TransactionsResponse>.Success(new TransactionsResponse(response));
        }
        else
        {
            var transaction = new Transaction(
                userId, 
                request.Title, 
                request.Description, 
                request.Type, 
                request.TotalAmount, 
                request.PaidOrReceivedAt, 
                category.Id,
                InstallmentInfo.Single());

            await _transactionRepository.AddAsync(transaction, ct);

            await _unit.SaveChangesAsync(ct);

            var response = new TransactionsResponse(
                [
                    new TransactionResponse(
                    transaction.Id,
                    transaction.Title,
                    transaction.Description,
                    transaction.Type,
                    transaction.Amount,
                    transaction.PaidOrReceivedAt,
                    new CategorySummaryResponse(category.Id, category.Name, category.Color),
                    null)
                ]
            );

            return Result<TransactionsResponse>.Success(response);
        }
    }

    private static (decimal baseValue, decimal remainder) CalculateInstallments(decimal total, int count)
    {
        var baseValue = Math.Floor((total / count) * 100) / 100;
        var remainder = total - (baseValue * count);
        return (baseValue, remainder);
    }
}