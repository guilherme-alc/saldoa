using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Saldoa.API.Extensions;

internal static class ClaimsPrincipalExtensions
{
    internal static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("UserId não encontrado no token.");

        return userId;
    }

    internal static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out userId);
    }
}
