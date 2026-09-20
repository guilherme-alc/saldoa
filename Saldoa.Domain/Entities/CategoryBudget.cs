using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{

    public class CategoryBudget
    {
        private CategoryBudget() { }

        public CategoryBudget(long categoryId, DateOnly periodStart, DateOnly periodEnd, decimal limitAmount, Guid workspaceId)
        {
            if (workspaceId == Guid.Empty)
                throw new DomainException("Workspace inválido.");

            CategoryId = EnsureValidCategoryId(categoryId);
            (PeriodStart, PeriodEnd) = EnsureValidPeriod(periodStart, periodEnd);
            LimitAmount = EnsureValidLimit(limitAmount);
            WorkspaceId = workspaceId;
            CreatedAt = DateTimeOffset.UtcNow;
        }
        public long Id { get; }
        public long CategoryId { get; private set; }
        public string UserId { get; private set; } = null!;
        public Guid WorkspaceId { get; private set; }
        public DateOnly PeriodStart { get; private set; }
        public DateOnly PeriodEnd { get; private set; }
        public decimal LimitAmount { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Category Category { get; private set; } = default!;
    
        private static long EnsureValidCategoryId(long categoryId)
        {
            if (categoryId <= 0) 
                throw new DomainException("Categoria inválida.");
        
            return categoryId;
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