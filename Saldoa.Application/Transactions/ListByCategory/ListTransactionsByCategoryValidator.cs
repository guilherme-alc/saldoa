using FluentValidation;

namespace Saldoa.Application.Transactions.ListByCategory;

public class ListTransactionsByCategoryValidator : AbstractValidator<ListTransactionsByCategoryRequest>
{
    public ListTransactionsByCategoryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x)
            .Must(x =>
                !x.StartDate.HasValue ||
                !x.EndDate.HasValue ||
                x.EndDate >= x.StartDate)
            .WithMessage("A data final não pode ser menor que a data inicial.");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Id da categoria inválido");
    }
}