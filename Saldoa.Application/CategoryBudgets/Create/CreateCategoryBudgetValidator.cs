using System.Data;
using FluentValidation;

namespace Saldoa.Application.CategoryBudgets.Create;

public class CreateCategoryBudgetValidator : AbstractValidator<CreateCategoryBudgetRequest>
{
    public CreateCategoryBudgetValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Id da categoria inválido");
        
        RuleFor(x => x.LimitAmount)
            .GreaterThan(0)
            .WithMessage("Limit da categoria deve ser maior que 0");
        
        RuleFor(x => x)
            .Must(x =>
                x.PeriodEnd >= x.PeriodStart)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}