using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;

namespace Saldoa.Domain.Entities
{
    public class WorkspaceMembership
    {
        private WorkspaceMembership() { }
        public WorkspaceMembership(Guid workspaceId, Guid userId, WorkspaceRole role)
        {
            if (workspaceId == Guid.Empty)
                throw new DomainException("Workspace inválido.");

            if (userId == Guid.Empty)
                throw new DomainException("Usuário inválido ou não informado.");

            Id = Guid.CreateVersion7();
            WorkspaceId = workspaceId;
            UserId = userId;
            Role = EnsureValidRole(role);
            JoinedAt = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid WorkspaceId { get; private set; }
        public Guid UserId { get; private set; }
        public WorkspaceRole Role { get; private set; }
        public DateTimeOffset JoinedAt { get; private set; }
        public Workspace Workspace { get; private set; } = null!;

        internal void ChangeRole(WorkspaceRole role) => Role = EnsureValidRole(role);

        private static WorkspaceRole EnsureValidRole(WorkspaceRole role)
        {
            if (!Enum.IsDefined(role))
                throw new DomainException("Tipo de função inválida.");

            return role;
        }

    }
}
