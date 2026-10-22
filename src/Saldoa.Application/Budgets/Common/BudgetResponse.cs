using Saldoa.Domain.Enums;

namespace Saldoa.Application.Budgets.Common;

public sealed record BudgetResponse(long Id, long? CategoryId, BudgetScope Scope, DateOnly PeriodStart, DateOnly PeriodEnd, decimal LimitAmount);