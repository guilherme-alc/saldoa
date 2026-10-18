using FluentValidation;
using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Budgets.ListBudgets;

public class ListBudgetsValidator : AbstractValidator<ListBudgetsRequest>
{
    public ListBudgetsValidator()
    {
        this.AddPaginationRules();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}