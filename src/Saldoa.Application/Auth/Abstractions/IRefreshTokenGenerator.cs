namespace Saldoa.Application.Auth.Abstractions;


public sealed record RefreshTokenResult(string RawToken, DateTime ExpiresAt);

public interface IRefreshTokenGenerator
{
    RefreshTokenResult Generate();
}