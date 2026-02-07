namespace Saldoa.Application.CategoryBudgets.Create;

public sealed record CreateCategoryBudgetRequest(long CategoryId,  DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);
