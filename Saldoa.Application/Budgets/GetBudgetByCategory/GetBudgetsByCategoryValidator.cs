using FluentValidation;
using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Budgets.GetBudgetByCategory;

public class GetBudgetsByCategoryValidator : AbstractValidator<GetBudgetsByCategoryRequest>
{
    public GetBudgetsByCategoryValidator()
    {
        this.AddPaginationRules();
    }
}