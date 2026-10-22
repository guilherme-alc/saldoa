namespace Saldoa.Application.Auth.PasswordReset
{
    public sealed record ResetPasswordRequest(Guid UserId, string EncodedToken, string NewPassword);
}
