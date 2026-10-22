using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{

    public class Budget
    {
        private Budget() { }

        public Budget(long? categoryId, BudgetScope scope, DateOnly periodStart, DateOnly periodEnd, decimal limitAmount, Guid workspaceId)
        {
            if (workspaceId == Guid.Empty)
                throw new DomainException("Workspace inválido.");

            (CategoryId, Scope) = EnsureValidScope(categoryId, scope);
            (PeriodStart, PeriodEnd) = EnsureValidPeriod(periodStart, periodEnd);
            LimitAmount = EnsureValidLimit(limitAmount);
            WorkspaceId = workspaceId;
            CreatedAt = DateTimeOffset.UtcNow;
        }
        public long Id { get; }
        public long? CategoryId { get; private set; }
        public BudgetScope Scope { get; private set; }
        public Guid WorkspaceId { get; private set; }
        public DateOnly PeriodStart { get; private set; }
        public DateOnly PeriodEnd { get; private set; }
        public decimal LimitAmount { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Category Category { get; private set; } = default!;
    
        private static (long?, BudgetScope) EnsureValidScope(long? categoryId, BudgetScope scope)
        {
            if (!Enum.IsDefined(scope))
                throw new DomainException("Tipo da escopo inválido.");

            if (categoryId is null && scope == BudgetScope.Category)
                throw new DomainException("Categoria não informada.");

            if (categoryId is not null && categoryId <= 0 && scope == BudgetScope.Category) 
                throw new DomainException("Categoria inválida.");
        
            if(categoryId is not null && scope == BudgetScope.General)
                throw new DomainException("Não é possível que o escopo seja genérico com uma categoria informada.");

            return (categoryId, scope);
        }
    
        public void ChangeLimit(decimal limitAmount)
        {
            LimitAmount = EnsureValidLimit(limitAmount);
        }
        private static decimal EnsureValidLimit(decimal limitAmount)
        {
            if (limitAmount <= 0)
                throw new DomainException("Limite da categoria deve ser maior que 0.");

            return limitAmount;
        }

        public void ChangePeriod(DateOnly start, DateOnly end)
        {
            (PeriodStart, PeriodEnd) = EnsureValidPeriod(start, end);
        }
        private static (DateOnly start, DateOnly end) EnsureValidPeriod(DateOnly start, DateOnly end)
        {
            if (start > end)
                throw new DomainException("O início do período não pode ser maior que o fim.");

            return (start, end);
        }
    }
}