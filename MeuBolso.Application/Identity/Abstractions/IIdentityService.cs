namespace MeuBolso.Application.Identity.Abstractions;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email, CancellationToken ct);
    Task<string> CreateUserAsync(string email, string password, string? fullName, CancellationToken ct);
    Task<string?> ValidateCredentialsAndGetUserIdAsync(string email, string password, CancellationToken ct);
    Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default);
}