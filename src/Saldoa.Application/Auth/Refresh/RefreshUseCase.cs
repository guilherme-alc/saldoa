using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Refresh;

public sealed class RefreshUseCase(
    IRefreshTokenRepository refreshRepository,
        IJwtProvider jwtProvider,
        IIdentityService identityService,
        IRefreshTokenGenerator refreshTokenService,
        IUnitOfWork unitOfWork)
    {
    public async Task<Result<AuthResult>> ExecuteAsync(string refreshTokenRaw, CancellationToken ct)
    {
        var hash = RefreshToken.HashToken(refreshTokenRaw);
        var refreshToken = await refreshRepository.GetByHashAsync(hash, ct);

        if (refreshToken is null)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        if (refreshToken.IsRevoked)
        {
            if (!string.IsNullOrWhiteSpace(refreshToken.ReplacedByTokenHash))
            {
                await refreshRepository.RevokeTokenFamilyAsync(refreshToken.ReplacedByTokenHash, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }

            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        if (refreshToken.IsExpired)
        {
            var error = AuthErrors.InvalidAccess;
            return Result<AuthResult>.Failure(error);
        }

        var result = await identityService.GetEmailByUserIdAsync(refreshToken.UserId, ct);
        if (!result.IsSuccess)
        {
            return Result<AuthResult>.Failure(result.Error!);
        }

        var newRefresh = refreshTokenService.Generate();

        var newEntity = new RefreshToken(newRefresh.RawToken, refreshToken.UserId, newRefresh.ExpiresAt);
        refreshToken.Revoke(replacedByTokenHash: newEntity.TokenHash);
        await refreshRepository.AddAsync(newEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var newAccessResult = jwtProvider.CreateAccessToken(
            userId: refreshToken.UserId,
            email: result.Value!,
            claims: []);

        return Result<AuthResult>.Success(new AuthResult(
            AccessToken: newAccessResult.Token,
            RefreshToken: newRefresh.RawToken,
            AccessTokenExpiresAt: newAccessResult.ExpiresAt,
            RefreshTokenExpiresAt: newRefresh.ExpiresAt
        ));
    }
}
