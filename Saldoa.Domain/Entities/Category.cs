using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{
    public class Category
    {
        private Category() { }
        public Category(string userId, string name, string? description, string? color)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido.");

            Name = EnsureValidName(name);
            NormalizedName = Name.ToUpperInvariant();
            Description = EnsureValidDescription(description);
            Color = EnsureValidColor(color);
            UserId = userId;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public long Id { get; }
        public string Name { get; private set; } = null!;
        public string NormalizedName { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? Color { get; private set; }
        public string UserId { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        
        public void Rename(string name)
        {
            Name = EnsureValidName(name);
            NormalizedName = Name.ToUpperInvariant();
        }
        private static string EnsureValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Nome da categoria é obrigatório.");

            return name.Trim();
        }

        public void ChangeDescription(string? description)
        {
            Description = EnsureValidDescription(description);
        }
        private static string? EnsureValidDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public void ChangeColor(string? color)
        {
            Color = EnsureValidColor(color);
        }
        private static string? EnsureValidColor(string? color)
        {
            return string.IsNullOrWhiteSpace(color) ? null : color.Trim();
        }
    }
}