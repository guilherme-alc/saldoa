using System.Data;
using FluentValidation;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Budgets.Create;

public class CreateBudgetValidator : AbstractValidator<CreateBudgetRequest>
{
    public CreateBudgetValidator()
    {
        RuleFor(x => x.Scope)
            .IsInEnum()
            .WithMessage("Escopo do orçamento inválido. Valores válidos: general, category.");

        When(x => x.Scope == BudgetScope.Category, () =>
        {
            RuleFor(x => x.CategoryId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Id da categoria inválido");
        });


        When(x => x.Scope == BudgetScope.General, () =>
        {
            RuleFor(x => x.CategoryId)
                .Null()
                .WithMessage("Não é possível definir uma categoria para orçamentos gerais.");
        });

        RuleFor(x => x.LimitAmount)
            .GreaterThan(0)
            .WithMessage("Limite da categoria deve ser maior que 0");
        
        RuleFor(x => x)
            .Must(x =>
                x.PeriodEnd >= x.PeriodStart)
            .WithMessage("A data final não pode ser menor que a data inicial.");
    }
}