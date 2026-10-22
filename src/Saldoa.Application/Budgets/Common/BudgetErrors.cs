using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Budgets.Common
{
    public static class BudgetErrors
    {
        public static Error NotFound =>
            new("Budget.NotFound", "Orçamento da categoria não encontrado", ErrorType.NotFound);
        public static Error AlreadyExists =>
            new("Budget.AlreadyExists", $"Já existe um orçamento para essa categoria no período informado", ErrorType.Conflict);
        public static Error ClosedPeriod =>
            new(
                "Budget.ClosedPeriod",
                "Não é possível alterar um orçamento já encerrado",
                ErrorType.Conflict
            );
        public static Error Invalid =>
            new(
                "Budget.Invalid",
                "Não é possível associar a uma gategoria um orçamento geral",
                ErrorType.Conflict
            );
    }
}
