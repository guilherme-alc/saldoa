using Microsoft.Extensions.Time.Testing;
using Saldoa.Domain.Entities;
using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Saldoa.UnitTests.Domain.Entities
{
    public class WorkspaceInvitationTests
    {
        private static readonly string _token = WorkspaceInvitation.GenerateToken();
        private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
        public static readonly TheoryData<string?> BlankStrings = new()
        {
            "",
            (string?)null,
            " ",
            "\t",
            "\n"
        };

        #region Token
        [Fact]
        public void WhenGeneratingToken_ThenReturns43Base64UrlCharacters()
        {
            var token = WorkspaceInvitation.GenerateToken();

            Assert.Matches(@"\A[A-Za-z0-9_-]{43}\z", token);
        }
        #endregion

        #region Constructor
        [Fact]
        public void GivenValidData_WhenCreatingWorkspaceInvitation_ThenInitializesProperties()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.UtcNow.AddDays(7)
                .ToOffset(TimeSpan.FromHours(-3));

            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token,
                role,
                invitedByUserId, 
                expiresAt);

            var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(_token)));

            Assert.Equal(email, workspaceInvitation.Email);
            Assert.Equal(invitedByUsername, workspaceInvitation.InvitedByUsername);
            Assert.Equal(workspaceId, workspaceInvitation.WorkspaceId);
            Assert.Equal(role, workspaceInvitation.Role);
            Assert.Equal(WorkspaceInvitationStatus.Pending, workspaceInvitation.Status);
            Assert.Equal(invitedByUserId, workspaceInvitation.InvitedByUserId);
            Assert.Equal(expiresAt, workspaceInvitation.ExpiresAt);
            Assert.Equal(TimeSpan.Zero, workspaceInvitation.ExpiresAt.Offset);
            Assert.NotNull(workspaceInvitation.TokenHash);
            Assert.Equal(expectedHash, workspaceInvitation.TokenHash);
            Assert.Matches(@"^(0x)?[0-9a-fA-F]+$", workspaceInvitation.TokenHash);
            Assert.Null(workspaceInvitation.AcceptedByUserId);
            Assert.Null(workspaceInvitation.AcceptedAt);
        }

        [Fact]
        public void GivenEmptyWorkspaceId_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.Empty;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invitedByUsername,
                    workspaceId, 
                    _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Fact]
        public void GivenEmptyUserId_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.Empty;
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invitedByUsername,
                    workspaceId, 
                    _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankToken_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException(string? blankToken)
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invitedByUsername,
                    workspaceId, 
                    blankToken!, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankEmail_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException(string? blankEmail)
        {
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    blankEmail!, 
                    invitedByUsername,
                    workspaceId, 
                    _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Theory]
        [InlineData("johnnycashtest.com")]
        [InlineData("johnnycashtest")]
        [InlineData("johnnycash@test")]
        [InlineData("johnnycash@.com")]
        [InlineData("johnny cash@test.com")]
        [InlineData("johnnycash @test.com")]
        [InlineData("johnnycash@test.com\n")]
        public void GivenInvalidEmail_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException(string invalidEmail)
        {
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    invalidEmail, 
                    invitedByUsername,
                    workspaceId,
                    _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankInvitedByUsername_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException(string? invalidInvitedByUsername)
        {
            var email = "johnnycash@test.com";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invalidInvitedByUsername!,
                    workspaceId, 
                    _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Fact]
        public void GivenInvalidWorkspaceRole_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invitedByUsername,
                    workspaceId, 
                    _token, 
                    (WorkspaceRole)99,
                    invitedByUserId, 
                    expiresAt)
            );
        }

        [Fact]
        public void GivenInvalidExpiresAt_WhenCreatingWorkspaceInvitation_ThenThrowsDomainException()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var invitedByUserId = Guid.NewGuid();
            var invalidExpiresAt = DateTimeOffset.UtcNow.AddHours(-1);

            Assert.Throws<DomainException>(() =>
                new WorkspaceInvitation(
                    email, 
                    invitedByUsername,
                    workspaceId, _token, 
                    WorkspaceRole.Member,
                    invitedByUserId, 
                    invalidExpiresAt)
            );
        }
        #endregion

        #region Accept
        [Fact]
        public void GivenValidInvitation_WhenAccepting_ThenAcceptedStatus()
        {
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            workspaceInvitation.Accept(userId);

            Assert.Equal(WorkspaceInvitationStatus.Accepted, workspaceInvitation.Status);
            Assert.Equal(userId, workspaceInvitation.AcceptedByUserId);
            Assert.NotNull(workspaceInvitation.AcceptedAt);
        }

        [Fact]
        public void GivenInvalidUserId_WhenAccepting_ThenThrowsDomainException()
        {
            var userId = Guid.Empty;
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            Assert.Throws<DomainException>(() => workspaceInvitation.Accept(userId));

            Assert.Equal(WorkspaceInvitationStatus.Pending, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenAcceptedInvitation_WhenAcceptingAgain_ThenThrowsDomainException()
        {
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            workspaceInvitation.Accept(userId);

            Assert.Throws<DomainException>(() => workspaceInvitation.Accept(userId));
        }

        [Fact]
        public void GivenPendingInvitationAtDeadline_WhenAccepting_ThenThrowsDomainException()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);

            _clock.Advance(TimeSpan.FromMinutes(5));

            Assert.Throws<DomainException>(() => workspaceInvitation.Accept(userId));
            Assert.Equal(WorkspaceInvitationStatus.Pending, workspaceInvitation.Status);
            Assert.Null(workspaceInvitation.AcceptedByUserId);
            Assert.Null(workspaceInvitation.AcceptedAt);
        }
        
        [Fact]
        public void GivenInvitationMarkedExpired_WhenAccepting_ThenThrowsDomainException()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);
            _clock.Advance(TimeSpan.FromMinutes(5));

            workspaceInvitation.Expired();

            Assert.Throws<DomainException>(() => workspaceInvitation.Accept(userId));
        }
        #endregion

        #region Decline
        [Fact]
        public void GivenValidInvitation_WhenDeclining_ThenDeclinedStatus()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(email, invitedByUsername,
                workspaceId, _token, role,
                invitedByUserId, expiresAt);

            workspaceInvitation.Decline();

            Assert.Equal(WorkspaceInvitationStatus.Declined, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenAcceptedInvitation_WhenDeclining_ThenThrowsDomainException()
        {
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(email, invitedByUsername,
                workspaceId, _token, role,
                invitedByUserId, expiresAt);
            workspaceInvitation.Accept(userId);

            Assert.Throws<DomainException>(workspaceInvitation.Decline);
            Assert.Equal(WorkspaceInvitationStatus.Accepted, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenPendingInvitationAtDeadline_WhenDeclining_ThenThrowsDomainException()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);
            _clock.Advance(TimeSpan.FromMinutes(5));

            Assert.Throws<DomainException>(workspaceInvitation.Decline);
            Assert.Equal(WorkspaceInvitationStatus.Pending, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenInvitationMarkedExpired_WhenDeclining_ThenThrowsDomainException()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);
            _clock.Advance(TimeSpan.FromMinutes(5));

            workspaceInvitation.Expired();

            Assert.Throws<DomainException>(workspaceInvitation.Decline);
        }
        #endregion

        #region Revoke
        [Fact]
        public void GivenValidInvitation_WhenRevoking_ThenRevokedStatus()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            workspaceInvitation.Revoke(invitedByUserId);

            Assert.Equal(WorkspaceInvitationStatus.Revoked, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenAnotherUser_WhenRevoking_ThenThrowsDomainException()
        {
            var anotherUserId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            Assert.Throws<DomainException>(() => workspaceInvitation.Revoke(anotherUserId));
        }

        [Fact]
        public void GivenInvalidUserId_WhenRevoking_ThenThrowsDomainException()
        {
            var userId = Guid.Empty;
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token,
                role,
                invitedByUserId, 
                expiresAt);

            Assert.Throws<DomainException>(() => workspaceInvitation.Revoke(userId));
        }

        [Fact]
        public void GivenInvitationStatus_WhenRevoking_ThenThrowsDomainException()
        {
            var userId = Guid.NewGuid();
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt);

            workspaceInvitation.Accept(userId);

            Assert.Throws<DomainException>(() => workspaceInvitation.Revoke(invitedByUserId));
            Assert.Equal(WorkspaceInvitationStatus.Accepted, workspaceInvitation.Status);
        }
        #endregion

        #region Expired
        [Fact]
        public void GivenValidInvitation_WhenExpiring_ThenExpiredStatus()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token,
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);
            _clock.Advance(TimeSpan.FromMinutes(5));

            workspaceInvitation.Expired();

            Assert.Equal(WorkspaceInvitationStatus.Expired, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenRevokedInvitationStatus_WhenExpiring_ThenThrowsDomainException()
        {
            var expiresAt = _clock.GetUtcNow().AddMinutes(5);
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var workspaceInvitation = new WorkspaceInvitation(
                email, 
                invitedByUsername,
                workspaceId, 
                _token, 
                role,
                invitedByUserId, 
                expiresAt,
                timeProvider: _clock);
            workspaceInvitation.Revoke(invitedByUserId);
            _clock.Advance(TimeSpan.FromMinutes(5));

            Assert.Throws<DomainException>(workspaceInvitation.Expired);
            Assert.Equal(WorkspaceInvitationStatus.Revoked, workspaceInvitation.Status);
        }

        [Fact]
        public void GivenValidInvitation_WhenExpiring_ThenThrowsDomainException()
        {
            var email = "johnnycash@test.com";
            var invitedByUsername = "johnny Doe";
            var workspaceId = Guid.NewGuid();
            var role = WorkspaceRole.Member;
            var invitedByUserId = Guid.NewGuid();
            var expiresAt = DateTimeOffset.Now.AddDays(7);
            var workspaceInvitation = new WorkspaceInvitation(email, invitedByUsername,
                workspaceId, _token, role,
                invitedByUserId, expiresAt);

            Assert.Throws<DomainException>(workspaceInvitation.Expired);
            Assert.Equal(WorkspaceInvitationStatus.Pending, workspaceInvitation.Status);
        }
        #endregion

    }
}
