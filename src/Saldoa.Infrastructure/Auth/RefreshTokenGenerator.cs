using Saldoa.Application.Auth.Abstractions;
using Microsoft.Extensions.Options;
using Saldoa.Domain.Auth;

namespace Saldoa.Infrastructure.Auth;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private readonly JwtOptions _options;
    public RefreshTokenGenerator(IOptions<JwtOptions> options) => _options = options.Value;

    public RefreshTokenResult Generate()
    {
        var raw = RefreshToken.GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
        return new RefreshTokenResult(raw, expiresAt);
    }
}