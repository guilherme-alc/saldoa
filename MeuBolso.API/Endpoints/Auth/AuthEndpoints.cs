using MeuBolso.Application.Auth.Logout;
using MeuBolso.Application.Auth.Refresh;

namespace MeuBolso.API.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth")
            .AllowAnonymous();
        
        RegisterEndpoint.Map(group);
        LoginEndpoint.Map(group);
        LogoutEndpoint.Map(group);
        RefreshEndpoint.Map(group);
    }
}