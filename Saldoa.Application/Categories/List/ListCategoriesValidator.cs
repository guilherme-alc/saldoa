using FluentValidation;

namespace Saldoa.Application.Categories.List;

public sealed class ListCategoriesValidator : AbstractValidator<ListCategoriesRequest>
{
    public ListCategoriesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
    }
}