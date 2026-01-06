using MeuBolso.Application.Auth;
using MeuBolso.Application.Auth.Abstractions;
using Microsoft.Extensions.Options;

namespace MeuBolso.API.Auth;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly JwtOptions _options;
    public RefreshTokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public RefreshTokenResult Generate()
    {
        var raw = RefreshTokenCrypto.GenerateToken();
        var hash = RefreshTokenCrypto.HashToken(raw);
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
        return new RefreshTokenResult(raw, hash, expiresAt);
    }
}