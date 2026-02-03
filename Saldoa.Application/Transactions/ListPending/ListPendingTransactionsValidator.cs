using FluentValidation;

namespace Saldoa.Application.Transactions.ListPending;

public class ListPendingTransactionsValidator : AbstractValidator<ListPendingTransactionsRequest>
{
    public ListPendingTransactionsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
        
        When(x => x.Type is not null, () =>
        {
            RuleFor(x => x.Type!.Value)
                .IsInEnum()
                .WithMessage("Tipo da transação inválido.");
        });
    }
}