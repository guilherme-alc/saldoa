using FluentValidation;

namespace Saldoa.Application.CategoryBudgets.Update;

public class UpdateCategoryBudgetValidator : AbstractValidator<UpdateCategoryBudgetRequest>
{
    public UpdateCategoryBudgetValidator()
    {
        RuleFor(x => x.LimitAmount)
            .GreaterThan(0)
            .WithMessage("Limite da categoria deve ser maior que 0");

        RuleFor(x => x)
            .Must(x =>
                x.PeriodEnd >= x.PeriodStart)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}