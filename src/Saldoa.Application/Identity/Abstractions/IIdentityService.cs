using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Identity.Abstractions;

public interface IIdentityService
{
    Task<bool> UserExistsAsync(string email, CancellationToken ct);
    Task<Result<CreateUserResult>> CreateUserAsync(string email, string password, string firstName, string? lastName, CancellationToken ct);
    Task<Result<Guid>> SignInAsync(string email, string password, CancellationToken ct);
    Task<Result<string?>> GetEmailByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Result> ConfirmEmailAsync(Guid userId, string encodedToken, CancellationToken ct);
    Task<Result> UpdateLastConfirmationEmailSentAtAsync(Guid userId, CancellationToken ct);
    Task<Result<ConfirmationTokenResult>> PrepareEmailConfirmationResendAsync(string email, CancellationToken ct);
    Task<Result<ResetPasswordTokenResult>> GenerateResetPasswordTokenAsync(string email, CancellationToken ct);
    Task<Result> UpdateLastPasswordResetEmailSentAtAsync(Guid userId, CancellationToken ct);
    Task<Result> ResetPasswordAsync(Guid userId, string encodedToken, string newPassword, CancellationToken ct);
}
