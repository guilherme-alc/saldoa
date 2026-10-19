using Saldoa.Domain.Entities;

namespace Saldoa.Application.Workspaces.Abstractions
{
    public interface IWorkspaceRepository
    {
        Task AddAsync(Workspace workspace, CancellationToken ct);
    }
}
