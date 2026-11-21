using Microsoft.Extensions.Time.Testing;
using Saldoa.Domain.Auth;
using Saldoa.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Saldoa.UnitTests.Domain.Auth
{
    public class RefreshTokenTests
    {
        private readonly FakeTimeProvider _cloak = new(new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero));
        private static readonly string _token = RefreshToken.GenerateToken();
        public static readonly TheoryData<string?> BlankStrings =
        [
            "",
            " ",
            (string?)null,
            "\n",
            "\t"
        ];
        public static readonly TheoryData<DateTimeOffset> InvalidDateTime =
        [
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            DateTimeOffset.Parse("2026-01-01T11:59:00Z"),
            DateTimeOffset.Parse("2026-01-01T12:00:00Z")
        ];


        [Fact]
        public void GivenValidData_WhenCreatingRefreshToken_ThenInitializesProperties()
        {
            var userId = Guid.NewGuid();
            var now = _cloak.GetUtcNow();
            var expiresAt = now.AddMinutes(15);
            var expiresAtWithOffset = expiresAt.ToOffset(TimeSpan.FromHours(-3));
            var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(_token)));

            var refreshToken = new RefreshToken(_token, userId, expiresAtWithOffset, _cloak);
            
            Assert.Matches(@"^(0x)?[0-9a-fA-F]+$", refreshToken.TokenHash);
            Assert.Equal(expectedHash, refreshToken.TokenHash);
            Assert.Equal(userId, refreshToken.UserId);
            Assert.Equal(expiresAt, refreshToken.ExpiresAt);
            Assert.Equal(TimeSpan.Zero, refreshToken.ExpiresAt.Offset);
            Assert.Null(refreshToken.ReplacedByTokenHash);
            Assert.Null(refreshToken.RevokedAt);
            Assert.NotEqual(Guid.Empty, refreshToken.Id);
            Assert.NotEqual(DateTimeOffset.MinValue, refreshToken.CreatedAt);
            Assert.True(now <= refreshToken.CreatedAt);
        }

        [Fact]
        public void WhenClockReachesExpiresAt_ThenTokenIsExpired()
        {
            var userId = Guid.NewGuid();
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);
            var refreshToken = new RefreshToken(_token, userId, expiresAt, _cloak);
            var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(_token)));

            Assert.False(refreshToken.IsExpired);

            _cloak.Advance(TimeSpan.FromMinutes(15));

            Assert.True(refreshToken.IsExpired);
        }

        [Theory]
        [MemberData(nameof(BlankStrings))]
        public void GivenBlankToken_WhenCreatingRefreshToken_ThenThrowsDomainException(string? blankString)
        {
            var userId = Guid.NewGuid();
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);

            Assert.Throws<DomainException>(() => 
                new RefreshToken(blankString!, userId, expiresAt, _cloak));
        }

        [Fact]
        public void GivenEmptyUserId_WhenCreatingRefreshToken_ThenThrowsDomainException()
        {
            var userId = Guid.Empty;
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);

            Assert.Throws<DomainException>(() => 
                new RefreshToken(_token, userId, expiresAt, _cloak));
        }

        [Theory]
        [MemberData(nameof(InvalidDateTime))]
        public void GivenInvalidExpiresAt_WhenCreatingRefreshToken_ThenThrowsDomainException(DateTimeOffset invalidExpiresDate)
        {
            var userId = Guid.NewGuid();

            Assert.Throws<DomainException>(() => 
                new RefreshToken(_token, userId, invalidExpiresDate, _cloak));
        }

        [Fact]
        public void WhenGeneratingToken_ThenReturns86Base64UrlCharacters()
        {
            var token = RefreshToken.GenerateToken();

            Assert.Matches(@"\A[A-Za-z0-9_-]{86}\z", token);
        }

        [Fact]
        public void WhenRevokingRefreshToken_ThenRevoked()
        {
            var userId = Guid.NewGuid();
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);
            var refreshToken = new RefreshToken(_token, userId, expiresAt, _cloak);
            _cloak.Advance(TimeSpan.FromMinutes(5));
            refreshToken.Revoke();

            Assert.NotNull(refreshToken.RevokedAt);
            Assert.True(refreshToken.IsRevoked);
        }

        [Fact]
        public void GivenReplacedByTokenHash_WhenRevokingWithReplacementHash_ThenStoresReplacementHash()
        {
            var userId = Guid.NewGuid();
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);
            var refreshToken = new RefreshToken(_token, userId, expiresAt, _cloak);
            var tokenHash = RefreshToken.HashToken(RefreshToken.GenerateToken());
            refreshToken.Revoke(tokenHash);

            Assert.NotNull(refreshToken.RevokedAt);
            Assert.Equal(tokenHash, refreshToken.ReplacedByTokenHash);
            Assert.True(refreshToken.IsRevoked);
        }

        [Fact]
        public void GivenRevokedRefreshToken_WhenRevokingRefreshToken_ThenRevokedAtNotChanged()
        {
            var userId = Guid.NewGuid();
            var expiresAt = _cloak.GetUtcNow().AddMinutes(15);
            var refreshToken = new RefreshToken(_token, userId, expiresAt, _cloak);
            var tokenHash = RefreshToken.HashToken(RefreshToken.GenerateToken());
            refreshToken.Revoke(tokenHash);
            var revokedAt = refreshToken.RevokedAt;
            _cloak.Advance(TimeSpan.FromMinutes(5));

            var newTokenHash = RefreshToken.HashToken(RefreshToken.GenerateToken());
            refreshToken.Revoke(newTokenHash);

            Assert.NotNull(refreshToken.RevokedAt);
            Assert.Equal(revokedAt, refreshToken.RevokedAt);
            Assert.Equal(tokenHash, refreshToken.ReplacedByTokenHash);
        }
    }
}
