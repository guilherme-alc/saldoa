using MeuBolso.Domain.Enums;

namespace MeuBolso.Domain.Entities
{
    public class Transaction
    {
        private Transaction() { }
        public Transaction(
            string userId,
            string title, 
            string? description, 
            ETransactionType type, 
            decimal amount, 
            DateOnly? paidOrReceivedAt,
            long categoryId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Usuário inválido.", nameof(userId));
            UserId = userId;
            SetTitle(title);
            SetDescription(description);
            Type = type;
            SetAmount(amount);
            PaidOrReceivedAt = paidOrReceivedAt;
            CreatedAt = DateTimeOffset.UtcNow;
            CategoryId = categoryId;
        }
        public long Id { get; private set; }
        public string Title { get; private set; }= null!;
        public string? Description { get; private set; }
        public ETransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public DateOnly? PaidOrReceivedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public long CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;
        public string UserId { get; private set; } = null!;
        
        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Título da transação é obrigatório.", nameof(title));

            Title = title.Trim();
        }
        
        public void SetDescription(string? description)
        {
            var d = description?.Trim();
            Description = string.IsNullOrWhiteSpace(d) ? null : d;
        }
        
        public void SetAmount(decimal amount) => Amount = Math.Abs(amount);
    }
}