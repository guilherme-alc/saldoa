using Saldoa.Domain.Enums;

namespace Saldoa.Application.Common.Abstractions
{
    public interface IWorkspaceContext
    {
        Guid? WorkspaceId { get; set; }
        Guid? UserId { get; set; }
        WorkspaceRole? Role { get; set; }

        void Set(Guid workspaceId, Guid userId, WorkspaceRole role);
    }
}
