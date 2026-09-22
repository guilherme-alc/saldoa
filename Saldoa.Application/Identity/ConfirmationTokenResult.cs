namespace Saldoa.Application.Identity
{
    public sealed record ConfirmationTokenResult(
        bool ShouldSendEmail,
        string? Email,
        Guid? UserId,
        string? Token);
}
