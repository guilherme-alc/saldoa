using Saldoa.Domain.ValueObjects;

namespace Saldoa.Application.Transactions.Common
{
    public sealed record InstallmentDraft(
        decimal Amount,
        DateOnly Date,
        InstallmentInfo InstallmentInfo);
}
