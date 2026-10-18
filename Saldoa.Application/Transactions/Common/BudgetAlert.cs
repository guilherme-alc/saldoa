using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Common
{
    public sealed record BudgetAlert(
        long BudgetId,
        BudgetScope Scope,
        decimal CurrentSpent,
        decimal ProjectedSpent,
        decimal? Limit,
        string Message);
}