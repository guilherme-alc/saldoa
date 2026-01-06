namespace MeuBolso.Application.Identity.Abstractions;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email);
    Task<string> CreateUserAsync(string email, string password, string? fullName);
    Task<string?> ValidateCredentialsAndGetUserIdAsync(string email, string password);
    Task<string?> GetEmailByUserIdAsync(string userId);
}