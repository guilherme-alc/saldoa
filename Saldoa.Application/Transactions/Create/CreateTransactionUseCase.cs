using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.CategoryBudgets.Abstractions;
using Saldoa.Application.CategoryBudgets.Create;
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

        var totalInstallments = request.TotalInstallments.HasValue && request.TotalInstallments.Value > 0
            ? request.TotalInstallments.Value
            : 1;

        var installments = BuildInstallments(request, totalInstallments);

        if (request.Type == ETransactionType.Expense)
        {
            var validationResult = await ValidateCategoryBudgetAsync(
                userId,
                request.CategoryId,
                installments,
                ct);

            if (!validationResult.IsSuccess)
                return Result<TransactionsResponse>.Failure(validationResult.Error!);
        }

        var transactions = installments.Select(i =>
            new Transaction(
                userId,
                request.Title,
                request.Description,
                request.Type,
                i.Amount,
                i.Date,
                category.Id,
                i.InstallmentInfo
            )
        ).ToList();

        if (transactions.Count > 1)
            await _transactionRepository.AddRangeAsync(transactions, ct);
        else
            await _transactionRepository.AddAsync(transactions[0], ct);

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
                t.InstallmentInfo.TotalInstallments > 1 ? t.InstallmentInfo : null
            )
        ).ToList();

        return Result<TransactionsResponse>.Success(new TransactionsResponse(response));
    }

    private static List<InstallmentDraft> BuildInstallments(CreateTransactionRequest request, int totalInstallments)
    {
        var (baseValue, remainder) = CalculateInstallments(request.TotalAmount, totalInstallments);
        var groupId = totalInstallments > 1 ? Guid.CreateVersion7() : (Guid?)null;

        var installments = new List<InstallmentDraft>();

        for (int i = 1; i <= totalInstallments; i++)
        {
            var amount = i == totalInstallments ? baseValue + remainder : baseValue;
            var date = request.PaidOrReceivedAt?.AddMonths(i - 1);

            var installmentInfo = totalInstallments > 1
                ? InstallmentInfo.Create(totalInstallments, i, groupId!.Value)
                : InstallmentInfo.Single();

            installments.Add(new InstallmentDraft(amount, date, installmentInfo));
        }

        return installments;
    }

    private async Task<Result> ValidateCategoryBudgetAsync(
        string userId,
        long categoryId,
        List<InstallmentDraft> installments,
        CancellationToken ct)
    {
        var installmentsWithDate = installments
            .Where(i => i.Date.HasValue)
            .ToList();

        if (installmentsWithDate.Count == 0)
            return Result.Success();

        var amountsByBudgetPeriod = new Dictionary<(DateOnly Start, DateOnly End), decimal>();

        foreach (var installment in installmentsWithDate)
        {
            var date = installment.Date!.Value;

            var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                userId,
                categoryId,
                date,
                ct);

            if (budget is null)
                continue;

            var key = (budget.PeriodStart, budget.PeriodEnd);

            if (!amountsByBudgetPeriod.ContainsKey(key))
                amountsByBudgetPeriod[key] = 0;

            amountsByBudgetPeriod[key] += installment.Amount;
        }

        foreach (var period in amountsByBudgetPeriod)
        {
            var spent = await _transactionRepository.GetTotalForPeriodAsync(
                userId,
                categoryId,
                period.Key.Start,
                period.Key.End,
                ct,
                ETransactionType.Expense);

            var totalProjected = spent + period.Value;

            var budget = await _categoryBudgetRepository.GetActiveForPeriodAsync(
                userId,
                categoryId,
                period.Key.Start,
                ct);

            if (budget is not null && totalProjected > budget.LimitAmount)
            {
                return Result.Failure(
                    $"Limite excedido para o período {period.Key.Start:MM/yyyy}");
            }
        }

        return Result.Success();
    }

    private static (decimal baseValue, decimal remainder) CalculateInstallments(decimal total, int count)
    {
        var baseValue = Math.Floor((total / count) * 100) / 100;
        var remainder = total - (baseValue * count);
        return (baseValue, remainder);
    }
}