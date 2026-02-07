namespace Saldoa.Application.CategoryBudgets.Common;

public sealed record CategoryBudgetResponse(long Id, long CategoryId, DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);