using Microsoft.AspNetCore.Authorization;
using Saldoa.API.Extensions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.WorkspaceMemberships.Abstractions;

namespace Saldoa.API.Security
{
    public sealed class WorkspaceRoleHandler(
        IWorkspaceMembershipRepository membershipRepository, 
        IWorkspaceContext context) : AuthorizationHandler<WorkspaceRoleRequirement>
    {
        private readonly IWorkspaceMembershipRepository _membershipRepository = membershipRepository;
        private readonly IWorkspaceContext _context = context;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authCtx, WorkspaceRoleRequirement requirement)
        {
            if (authCtx.User.Identity?.IsAuthenticated != true ||
                authCtx.Resource is not HttpContext httpContext ||
                !authCtx.User.TryGetUserId(out var userId) ||
                httpContext.Request.RouteValues["workspaceId"] is not string raw ||
                !Guid.TryParse(raw, out var workspaceId))
                return;

            var membership = await _membershipRepository.GetAsync(workspaceId, userId, httpContext.RequestAborted);

            if (membership is null || membership.Role < requirement.WorkspaceRole)
                return;

            _context.Set(workspaceId, userId, membership.Role);

            authCtx.Succeed(requirement);
        }
    }
}
