using Microsoft.EntityFrameworkCore;
using Saldoa.Application.WorkspaceMemberships.Abstractions;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Repositories;

public class WorkspaceMembershipRepository(SaldoaDbContext dbContext) : IWorkspaceMembershipRepository
{
    public async Task<WorkspaceMembership?> GetAsync(Guid workspaceId, Guid userId, CancellationToken ct)
    {
        return await dbContext.WorkspaceMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId,  ct);
    }
}
