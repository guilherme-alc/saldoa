namespace Saldoa.Application.Transactions.Common
{
    public sealed record BudgetAlert(
        decimal CurrentSpent,
        decimal ProjectedSpent,
        decimal? Limit,
        string Message);
}