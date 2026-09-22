namespace Saldoa.Application.Auth.ConfirmEmail
{
    public sealed record ConfirmEmailRequest(Guid UserId, string EncodedToken);
}
