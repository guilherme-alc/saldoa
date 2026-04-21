using Saldoa.Domain.ValueObjects;

namespace Saldoa.Application.CategoryBudgets.Create
{
    public sealed record InstallmentDraft(
        decimal Amount,
        DateOnly Date,
        InstallmentInfo InstallmentInfo);
}
