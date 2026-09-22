using System.Security.Claims;

namespace Saldoa.Application.Auth.Abstractions;
public sealed record AccessTokenResult(string Token, DateTime ExpiresAt);
public interface IJwtProvider
{
    AccessTokenResult CreateAccessToken(Guid userId, string email, IEnumerable<Claim> claims);
}