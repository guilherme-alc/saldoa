namespace Saldoa.Application.CategoryBudgets.Common;

public sealed record CategoryBudgetDetailsResponse(
    long Id, 
    long CategoryId, 
    DateOnly PeriodStart, 
    DateOnly PeriodEnd, 
    decimal LimitAmount, 
    decimal TotalSpent, 
    decimal RemainingAmount,
    decimal PercentageUsed);