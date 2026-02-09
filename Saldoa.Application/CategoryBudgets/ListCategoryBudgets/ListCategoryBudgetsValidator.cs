using FluentValidation;

namespace Saldoa.Application.CategoryBudgets.ListCategoryBudgets;

public class ListCategoryBudgetsValidator : AbstractValidator<ListCategoryBudgetsRequest>
{
    public ListCategoryBudgetsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}