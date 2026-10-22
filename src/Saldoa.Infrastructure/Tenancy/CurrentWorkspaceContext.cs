using Saldoa.Application.Common.Abstractions;
using Saldoa.Domain.Enums;

namespace Saldoa.Infrastructure.Tenancy
{
    internal class CurrentWorkspaceContext : IWorkspaceContext
    {

        public Guid? WorkspaceId { get; set; }
        public Guid? UserId { get; set; }
        public WorkspaceRole? Role { get; set; }

        public void Set(Guid workspaceId, Guid userId, WorkspaceRole role)
        {
            if (WorkspaceId.HasValue)
                throw new InvalidOperationException(
                    "O workspace atual já foi definido.");
            if (UserId.HasValue)
                throw new InvalidOperationException(
                    "O usuário atual já foi definido.");

            WorkspaceId = workspaceId;
            UserId = userId;
            Role = role;
        }
    }
}
