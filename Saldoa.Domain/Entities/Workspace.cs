using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{
    public class Workspace
    {
        private Workspace() { }
        public Workspace(string name, string userId)
        {
            var validUserId = EnsureValidUserId(userId);
            Id = Guid.CreateVersion7();
            Name = EnsureValidName(name);
            _memberships = [CreateOwner(validUserId)];
            CreatedByUserId = validUserId;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string CreatedByUserId { get; private set; } = null!;
        public DateTimeOffset CreatedAt { get; private set; }
        private readonly List<WorkspaceMembership> _memberships = [];
        public IReadOnlyCollection<WorkspaceMembership> Memberships => _memberships.AsReadOnly();

        public void Rename(string name) => Name = EnsureValidName(name);
        
        public void AddMember(WorkspaceMembership member)
        {
            member = EnsureValidWorkspaceMembership(member);

            if (_memberships.Any(m => m.UserId == member.UserId))
                throw new DomainException("Usuário já está vinculado ao workspace.");

            _memberships.Add(member);
        }

        public void RemoveMember(string userId)
        {
            userId = EnsureValidUserId(userId);

            var existing = _memberships.FirstOrDefault(m => m.UserId == userId);
            if (existing == null)
                throw new DomainException("Vínculo do usuário não encontrado no workspace.");

            if (existing.Role == WorkspaceRole.Owner && _memberships.Count(m => m.Role == WorkspaceRole.Owner) <= 1)
                throw new DomainException("Não é possível remover o último proprietário do workspace.");

            _memberships.Remove(existing);
        }

        public void ChangeMemberRole(string userId, WorkspaceRole role)
        {
            userId = EnsureValidUserId(userId);
            if (!Enum.IsDefined(role))
                throw new DomainException("Tipo de função inválida.");

            var existing = _memberships.FirstOrDefault(m => m.UserId == userId);
            if (existing == null)
                throw new DomainException("Vínculo do usuário não encontrado no workspace.");

            if (existing.Role == WorkspaceRole.Owner && role != WorkspaceRole.Owner && _memberships.Count(m => m.Role == WorkspaceRole.Owner) == 1)
            {
                throw new DomainException("Não é possível alterar a função desse usuário. O workspace ficaria sem proprietário.");
            }

            existing.ChangeRole(role);
        }

        private WorkspaceMembership CreateOwner(string userId)
        {
            var owner = new WorkspaceMembership(Id, userId, WorkspaceRole.Owner);

            return EnsureValidWorkspaceMembership(owner);
        }

        private WorkspaceMembership EnsureValidWorkspaceMembership(WorkspaceMembership workspaceMembership)
        {
            if (workspaceMembership == null || workspaceMembership.Id == Guid.Empty || string.IsNullOrWhiteSpace(workspaceMembership.UserId))
                throw new DomainException("O vínculo do usuário não é válido");

            if (workspaceMembership.WorkspaceId != Id)
                throw new DomainException($"Esse membro não pertence ao workspace {Name}.");

            return workspaceMembership;
        }

        private static string EnsureValidUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainException("Usuário inválido ou não informado.");

            return userId.Trim();
        }

        private static string EnsureValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome é obrigatório.");

            return name.Trim();
        }
    }
}
