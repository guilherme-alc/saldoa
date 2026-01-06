namespace MeuBolso.Application.Auth.Abstractions;


public sealed record RefreshTokenResult(string RawToken, string TokenHash, DateTime ExpiresAt);

public interface IRefreshTokenService
{
    RefreshTokenResult Generate();
}