using Saldoa.Application.Workspaces.Abstractions;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Repositories
{
    public class WorkspaceRepository(SaldoaDbContext dbContext) : IWorkspaceRepository
    {
        public async Task AddAsync(Workspace workspace, CancellationToken ct)
        {
            await dbContext.Workspaces.AddAsync(workspace, ct);
        }
    }
}
