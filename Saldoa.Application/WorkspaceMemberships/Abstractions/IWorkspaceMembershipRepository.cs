using Saldoa.Domain.Entities;

namespace Saldoa.Application.WorkspaceMemberships.Abstractions
{
    public interface IWorkspaceMembershipRepository
    {
        Task<WorkspaceMembership?> GetAsync(Guid workspaceId, Guid userId, CancellationToken ct);
    }
}
