using Saldoa.Application.Auth.Abstractions;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Domain.Auth;

namespace Saldoa.Application.Auth.Login;

public class LoginUseCase(
    IIdentityService identityService,
    IJwtProvider jwtProvider,
    IRefreshTokenRepository refreshRepo,
    IRefreshTokenGenerator refreshTokenService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<AuthResult>> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var result = await identityService
            .SignInAsync(request.Email, request.Password, ct);

        if (!result.IsSuccess)
        {
            var error = result.Error ?? AuthErrors.InvalidAccess;
            if (error.Type == ErrorType.Unexpected)
                return Result<AuthResult>.Failure(error);

            return Result<AuthResult>.Failure(AuthErrors.InvalidAccess);
        }

        var accessTokenResult = jwtProvider.CreateAccessToken(
            userId: result.Value!,
            email: request.Email,
            claims: []);
        
        var refreshToken = refreshTokenService.Generate();
        
        var entityRefreshToken = new RefreshToken(refreshToken.RawToken, result.Value!, refreshToken.ExpiresAt);
        await refreshRepo.AddAsync(entityRefreshToken, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<AuthResult>.Success(
            new AuthResult(
                AccessToken: accessTokenResult.Token,
                RefreshToken: refreshToken.RawToken,
                AccessTokenExpiresAt: accessTokenResult.ExpiresAt,
                RefreshTokenExpiresAt: refreshToken.ExpiresAt
            )
        );
    }
}
