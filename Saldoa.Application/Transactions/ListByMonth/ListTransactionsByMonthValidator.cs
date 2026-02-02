using FluentValidation;

namespace Saldoa.Application.Transactions.ListByMonth;

public class ListTransactionsByMonthValidator : AbstractValidator<ListTransactionsByMonthRequest>
{
    public ListTransactionsByMonthValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Year)
            .GreaterThan(0)
            .WithMessage("Ano inválido.");
        
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("O mês deve estar entre 1 e 12.");
        
        When(x => x.Type is not null, () =>
        {
            RuleFor(x => x.Type!.Value)
                .IsInEnum()
                .WithMessage("Tipo da transação inválido.");
        });
    }
}