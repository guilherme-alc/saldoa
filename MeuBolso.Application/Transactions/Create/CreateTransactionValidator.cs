using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentValidation;

namespace MeuBolso.Application.Transactions.Create;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Title)
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .WithMessage("O título da transação é obrigatório")
            .MaximumLength(100)
            .WithMessage("O título não pode ultrapassar 100 caracteres");
        
        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("A descrição não pode ultrapassar 255 caracteres");

        RuleFor(x => x.Type)
            .IsInEnum()
            .Must(x => x != 0)
            .WithMessage("O tipo da transação é obrigatório");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("O valor da transação deve ser maior que 0");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("O id da categoria é obrigatório");
        
        var min = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3));
        var max = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2));

        When(x => x.PaidOrReceivedAt.HasValue, () =>
        {
            RuleFor(x => x.PaidOrReceivedAt!.Value)
                .InclusiveBetween(min, max)
                .WithMessage($"A data deve estar entre {min:dd/MM/yyyy} e {max:dd/MM/yyyy}.");
        });
    }
}