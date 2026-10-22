namespace Saldoa.Application.Identity
{
    public sealed record ResetPasswordTokenResult(
        bool ShouldSendEmail,
        string? Email,
        Guid? UserId,
        string? Token);
}
