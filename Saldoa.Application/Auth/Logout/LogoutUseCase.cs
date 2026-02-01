using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshRepo;

    public LogoutUseCase(IRefreshTokenRepository refreshRepo)
        => _refreshRepo = refreshRepo;

    public async Task<Result> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshTokenCrypto.HashToken(refreshTokenRaw);
        var stored = await _refreshRepo.GetByHashAsync(hash, ct);
        
        if (stored is null)
            return Result.Success();

        if (!stored.IsRevoked)
        {
            stored.Revoke();
            await _refreshRepo.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}