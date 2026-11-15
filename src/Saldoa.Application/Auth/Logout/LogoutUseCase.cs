using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Logout;

public sealed class LogoutUseCase(IRefreshTokenRepository refreshRepo, IUnitOfWork unitOfWork)
{
    public async Task<Result> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshToken.HashToken(refreshTokenRaw);
        var stored = await refreshRepo.GetByHashAsync(hash, ct);
        
        if (stored is null)
            return Result.Success();

        if (!stored.IsRevoked)
        {
            stored.Revoke();
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
