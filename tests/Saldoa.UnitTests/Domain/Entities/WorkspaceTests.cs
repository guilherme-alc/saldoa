using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class WorkspaceTests
    {
        public static readonly TheoryData<string?> BlankStrings = new()
        {
            "",
            (string?)null,
            " ",
            "\t",
            "\n"
        };

        #region Constructor
        [Fact]
        public void GivenValidData_WhenCreatingWorkspace_ThenInitializesProperties()
        {
            var name = " Casa ";
            var ownerId = Guid.NewGuid();

            var workspace = new Workspace(name, ownerId);

            var owner = Assert.Single(workspace.Memberships);
            Assert.Equal(ownerId, owner.UserId);
            Assert.Equal(WorkspaceRole.Owner, owner.Role);
            Assert.Equal(workspace.Id, owner.WorkspaceId);
            Assert.Equal("Casa", workspace.Name);
            Assert.Equal(ownerId, workspace.CreatedByUserId);
            Assert.True(workspace.Id != Guid.Empty);
            Assert.True(workspace.CreatedAt != DateTimeOffset.MinValue);
        }

        [Fact]
        public void GivenEmptyUserId_WhenCreatingWorkspace_ThenThrowsDomainException()
        {
            var name = "Casa";
            var emptyUserId = Guid.Empty;

            Assert.Throws<DomainException>(() => new Workspace(name, emptyUserId));
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankName_WhenCreatingWorkspace_ThenThrowsDomainException(string? invalidName)
        {
            var ownerId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => new Workspace(invalidName!, ownerId));
        }
        #endregion

        [Fact]
        public void GivenValidName_WhenRenamingWorkspace_ThenUpdatesName()
        {
            var name = "Casal";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var newName = " Casa ";

            workspace.Rename(newName);

            Assert.Equal("Casa", workspace.Name);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankName_WhenRenamingWorkspace_ThenThrowsDomainException(string? invalidName)
        {
            var name = "Casal";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);

            Assert.Throws<DomainException>(() => workspace.Rename(invalidName!));
            Assert.Equal(name, workspace.Name);
        }

        [Fact]
        public void GivenValidMember_WhenAddingMember_ThenMemberAdded()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var newMemberId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var membership = new WorkspaceMembership(workspace.Id, newMemberId, role);

            workspace.AddMember(membership);

            Assert.Contains(workspace.Memberships, m => m.UserId == newMemberId && m.Role == role);
        }

        [Fact]
        public void GivenExistingMember_WhenAddingMember_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var newMemberId = Guid.NewGuid();
            var membership = new WorkspaceMembership(workspace.Id, newMemberId, WorkspaceRole.Member);
            var membership2 = new WorkspaceMembership(workspace.Id, newMemberId, WorkspaceRole.ReadOnly);
            workspace.AddMember(membership);

            Assert.Throws<DomainException>(() => workspace.AddMember(membership));
            Assert.Throws<DomainException>(() => workspace.AddMember(membership2));
        }
        
        [Fact]
        public void GivenNullMembership_WhenAddingMember_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);

            Assert.Throws<DomainException>(() => workspace.AddMember(null!));
        }

        [Fact]
        public void GivenMembershipFromAnotherWorkspace_WhenAddingMember_ThenThrowsDomainException()
        {
            var name = "Casa 1";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var otherName = "Casa 2";
            var otherUserId = Guid.NewGuid();
            var otherWorkspace = new Workspace(otherName, otherUserId);
            var memberId = Guid.NewGuid();
            var membership = new WorkspaceMembership(otherWorkspace.Id, memberId, WorkspaceRole.Member);

            Assert.Throws<DomainException>(() => workspace.AddMember(membership));
        }

        [Fact]
        public void WhenRemovingMember_ThenMemberRemoved()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var readonlyMemberId = Guid.NewGuid();
            var role = WorkspaceRole.ReadOnly;
            var membership = new WorkspaceMembership(workspace.Id, readonlyMemberId, role);
            workspace.AddMember(membership);

            workspace.RemoveMember(readonlyMemberId);

            Assert.DoesNotContain(workspace.Memberships, m => m.UserId == readonlyMemberId);
            Assert.DoesNotContain(workspace.Memberships, o => o.Role == role);
        }

        [Fact]
        public void GivenEmptyUserId_WhenRemovingMember_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var emptyUserId = Guid.Empty;
            var countMembers = workspace.Memberships.Count;

            Assert.Throws<DomainException>(() => workspace.RemoveMember(emptyUserId));
            Assert.Equal(countMembers, workspace.Memberships.Count);
        }

        [Fact]
        public void GivenUnknownUserId_WhenRemovingMember_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var unknownUserId = Guid.NewGuid();
            var countMembers = workspace.Memberships.Count;

            Assert.Throws<DomainException>(() => workspace.RemoveMember(unknownUserId));
            Assert.Equal(countMembers, workspace.Memberships.Count);
        }

        [Fact]
        public void GivenMultipleOwners_WhenRemovingOneOwner_ThenOwnerRemoved()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var otherOwnerId = Guid.NewGuid();
            var workspaceMembership = new WorkspaceMembership(workspace.Id, otherOwnerId, WorkspaceRole.Owner);
            var countMembers = workspace.Memberships.Count;
            workspace.AddMember(workspaceMembership);

            workspace.RemoveMember(otherOwnerId);

            Assert.DoesNotContain(workspace.Memberships, m => m.UserId == otherOwnerId);
            Assert.Equal(countMembers, workspace.Memberships.Count);
        }

        [Fact]
        public void GivenOwnerMember_WhenRemovingOnlyOwner_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var memberId = Guid.NewGuid();
            var workspaceMembership = new WorkspaceMembership(workspace.Id, memberId, WorkspaceRole.Member);
            workspace.AddMember(workspaceMembership);

            Assert.Throws<DomainException>(() => workspace.RemoveMember(ownerId));
            var owner = Assert.Single(workspace.Memberships, m => m.Role == WorkspaceRole.Owner);
            Assert.Equal(ownerId, owner.UserId);
        }

        [Fact]
        public void GivenReadOnlyMember_WhenChangingRoleToMember_ThenRoleChanged()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var otherMemberId = Guid.NewGuid();
            var workspaceMembership = new WorkspaceMembership(workspace.Id, otherMemberId, WorkspaceRole.ReadOnly);
            workspace.AddMember(workspaceMembership);

            workspace.ChangeMemberRole(otherMemberId, WorkspaceRole.Member);

            Assert.Equal(WorkspaceRole.Member, workspaceMembership.Role);
        }

        [Fact]
        public void GivenOneOwner_WhenChangingRoleToOwner_ThenKeepsOwnerRole()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);

            workspace.ChangeMemberRole(ownerId, WorkspaceRole.Owner);

            var owner = Assert.Single(workspace.Memberships, m => m.Role == WorkspaceRole.Owner);
            Assert.Equal(ownerId, owner.UserId);
        }

        [Fact]
        public void GivenEmptyUserId_WhenChangingRole_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var emptyUserId = Guid.Empty;

            Assert.Throws<DomainException>(() => 
                workspace.ChangeMemberRole(emptyUserId, WorkspaceRole.Member));
        }

        [Fact]
        public void GivenInvalidRole_WhenChangingRole_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var otherMemberId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var workspaceMembership = new WorkspaceMembership(workspace.Id, otherMemberId, role);
            workspace.AddMember(workspaceMembership);

            Assert.Throws<DomainException>(() => 
                workspace.ChangeMemberRole(otherMemberId, (WorkspaceRole)99));
            Assert.Equal(role, workspaceMembership.Role);
        }

        [Fact]
        public void GivenUnknownUserId_WhenChangingRole_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var unknownUserId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => 
                workspace.ChangeMemberRole(unknownUserId, WorkspaceRole.Owner));
        }

        [Fact]
        public void GivenMultipleOwners_WhenDemotingOneOwner_ThenUpdatesRole()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var otherMemberId = Guid.NewGuid();
            var workspaceMembership = new WorkspaceMembership(workspace.Id, otherMemberId, WorkspaceRole.Owner);
            workspace.AddMember(workspaceMembership);

            workspace.ChangeMemberRole(otherMemberId, WorkspaceRole.Member);

            Assert.Equal(WorkspaceRole.Member, workspaceMembership.Role);
        }


        [Fact]
        public void GivenOneOwner_WhenDemotingThatOwner_ThenThrowsDomainException()
        {
            var name = "Casa";
            var ownerId = Guid.NewGuid();
            var workspace = new Workspace(name, ownerId);
            var memberId = Guid.NewGuid();
            var workspaceMembership = new WorkspaceMembership(workspace.Id, memberId, WorkspaceRole.Member);
            workspace.AddMember(workspaceMembership);

            Assert.Throws<DomainException>(() => 
                workspace.ChangeMemberRole(ownerId, WorkspaceRole.Member));
            var owner = Assert.Single(workspace.Memberships, m => m.Role == WorkspaceRole.Owner);
            Assert.Equal(ownerId, owner.UserId);
        }
    }
}
