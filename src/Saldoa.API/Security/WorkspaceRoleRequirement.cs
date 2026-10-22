using Microsoft.AspNetCore.Authorization;
using Saldoa.Domain.Enums;

namespace Saldoa.API.Security
{
    public class WorkspaceRoleRequirement : IAuthorizationRequirement
    {
        public WorkspaceRoleRequirement(WorkspaceRole minimum)
        {
            WorkspaceRole = minimum;
        }
        public WorkspaceRole WorkspaceRole { get; }
    }
}
