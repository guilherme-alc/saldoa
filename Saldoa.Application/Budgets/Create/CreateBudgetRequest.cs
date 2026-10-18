using Saldoa.Domain.Enums;

namespace Saldoa.Application.Budgets.Create;

public sealed record CreateBudgetRequest(long? CategoryId, BudgetScope Scope, DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);
