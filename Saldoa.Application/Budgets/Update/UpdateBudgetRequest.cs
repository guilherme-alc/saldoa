namespace Saldoa.Application.Budgets.Update;

public sealed record UpdateBudgetRequest(DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);