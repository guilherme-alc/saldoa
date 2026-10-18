using Saldoa.Domain.Enums;

namespace Saldoa.Application.Budgets.Common;

public sealed record BudgetDetailsResponse(
    long Id, 
    long? CategoryId, 
    BudgetScope Scope,
    DateOnly PeriodStart, 
    DateOnly PeriodEnd, 
    decimal LimitAmount, 
    decimal TotalSpent, 
    decimal RemainingAmount,
    decimal PercentageUsed);