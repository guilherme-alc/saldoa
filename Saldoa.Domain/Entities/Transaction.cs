using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;
using Saldoa.Domain.ValueObjects;

namespace Saldoa.Domain.Entities
{
    public class Transaction
    {
        private Transaction() { }
        public Transaction(
            Guid workspaceId,
            string title,
            string? description,
            TransactionType type,
            decimal totalAmount,
            DateOnly paidOrReceivedAt,
            long categoryId,
            InstallmentInfo installmentInfo,
            string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido.");

            if (workspaceId == Guid.Empty)
                throw new DomainException("Workspace inválido.");

            CreatedByUserId = userId;
            Title = EnsureValidTitle(title);
            Description = EnsureValidDescription(description);
            Amount = EnsureValidAmount(totalAmount);
            Type = EnsureValidType(type);
            PaidOrReceivedAt = paidOrReceivedAt;
            CreatedAt = DateTimeOffset.UtcNow;
            CategoryId = EnsureValidCategoryId(categoryId);
            InstallmentInfo = installmentInfo ?? throw new DomainException(nameof(installmentInfo));   
        }

        public long Id { get; }
        public string Title { get; private set; } = null!;
        public string? Description { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public DateOnly PaidOrReceivedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public long CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public InstallmentInfo InstallmentInfo { get; private set; } = null!;
        public string CreatedByUserId { get; private set; } = null!;
        public Guid WorkspaceId { get; private set; }

        public void ChangeTitle(string title)
        {
            Title = EnsureValidTitle(title);
        }
        private static string EnsureValidTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Título é obrigatório.");
            return title.Trim();
        }

        public void ChangeDescription(string? description)
        {
            Description = EnsureValidDescription(description);
        }
        private static string? EnsureValidDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public void ChangeAmount(decimal amount)
        {
            Amount = EnsureValidAmount(amount);
        }
        private static decimal EnsureValidAmount(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("O valor da transação deve ser maior que 0.");

            return amount;
        }

        public void Reschedule(DateOnly paidOrReceivedAt)
        {
            PaidOrReceivedAt = paidOrReceivedAt;
        }

        public void MoveToCategory(long categoryId)
        {
            CategoryId = EnsureValidCategoryId(categoryId);
        }
        private static long EnsureValidCategoryId(long categoryId)
        {
            if (categoryId <= 0)
                throw new DomainException("Categoria inválida.");

            return categoryId;
        }

        private static TransactionType EnsureValidType(TransactionType type)
        {
            if (!Enum.IsDefined(type))
                throw new DomainException("Tipo da transação inválido.");
            return type;
        }
    }
}