using Saldoa.Application.Auth.Logout;

namespace Saldoa.API.Endpoints.Auth;

public class LogoutEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/logout", async (
            HttpRequest http,
            LogoutRequest request,
            LogoutUseCase useCase,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.NoContent(); // NoContent por segurança

            await useCase.ExecuteAsync(request.RefreshToken, ct);
            return Results.NoContent();
        })
        .WithSummary("Encerra a sessão do usuário")
        .WithDescription(
            "Revoga o Refresh Token informado, invalidando futuras renovações de sessão. " +
            "Por segurança, sempre retorna 204 mesmo que o token não exista ou já esteja revogado."
        );
    }
}