using FluentValidation;

namespace Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

public class GetCategoryBudgetsByCategoryValidator : AbstractValidator<GetCategoryBudgetsByCategoryRequest>
{
    public GetCategoryBudgetsByCategoryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
    }
}