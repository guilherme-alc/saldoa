using Saldoa.Domain.Exceptions;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class WorkspaceMembershipTests
    {
        [Fact]
        public void GivenEmptyWorkspaceId_WhenCreatingWorkspaceMembership_ThenThrowsDomainException()
        {
            var workspaceId = Guid.Empty;
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() =>
                new WorkspaceMembership(workspaceId, userId, WorkspaceRole.Member));
        }

        [Fact]
        public void GivenEmptyUserId_WhenCreatingWorkspaceMembership_ThenThrowsDomainException()
        {
            var workspaceId = Guid.NewGuid();
            var userId = Guid.Empty;

            Assert.Throws<DomainException>(() =>
                new WorkspaceMembership(workspaceId, userId, WorkspaceRole.Member));
        }

        [Theory]
        [InlineData(WorkspaceRole.Owner)]
        [InlineData(WorkspaceRole.Member)]
        [InlineData(WorkspaceRole.ReadOnly)]
        public void GivenValidData_WhenCreatingWorkspaceMembership_ThenInitializesProperties(WorkspaceRole role)
        {
            var lowDateTime = DateTimeOffset.UtcNow;
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();
            var membership = new WorkspaceMembership(workspaceId, userId, role);

            Assert.NotEqual(Guid.Empty, membership.Id);
            Assert.Equal(workspaceId, membership.WorkspaceId);
            Assert.Equal(userId, membership.UserId);
            Assert.Equal(role, membership.Role);
            Assert.InRange(membership.JoinedAt, lowDateTime, DateTimeOffset.UtcNow);
            Assert.Equal(TimeSpan.Zero, membership.JoinedAt.Offset);
        }

        [Fact]
        public void GivenInvalidRole_WhenCreatingWorkspaceMembership_ThenThrowsDomainException()
        {
            var userId = Guid.NewGuid();
            var workspaceId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => 
                new WorkspaceMembership(workspaceId, userId, (WorkspaceRole)99));
        }
    }
}
