using Saldoa.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Saldoa.Domain.Auth;

public class RefreshToken
{
    private readonly TimeProvider _timeProvider = TimeProvider.System;
    private RefreshToken() { }

    public RefreshToken(string token, 
        Guid userId, 
        DateTimeOffset expiresAt, 
        TimeProvider? timeProvider = null)
    {
        if (timeProvider is not null)
            _timeProvider = timeProvider;

        var now = _timeProvider.GetUtcNow();

        if (userId == Guid.Empty)
            throw new DomainException("Usuário inválido");

        if (expiresAt == DateTimeOffset.MaxValue ||
            expiresAt == now ||
            expiresAt < now)
            throw new DomainException("Data de expiração inválida");

        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token inválido");

        TokenHash = HashToken(token);
        UserId = userId;
        ExpiresAt = expiresAt.ToUniversalTime();
        CreatedAt = now;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string TokenHash { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public bool IsRevoked => RevokedAt is not null;
    public bool IsExpired => _timeProvider.GetUtcNow() >= ExpiresAt;

    public static string GenerateToken() //client
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public static string HashToken(string token) //banco
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public void Revoke(string? replacedByTokenHash = null)
    {
        if (RevokedAt is not null)
            return;

        RevokedAt = _timeProvider.GetUtcNow();
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
