using Saldoa.Application.Auth.Refresh;

namespace Saldoa.API.Endpoints.Auth;

public static class RefreshEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/refresh", async (
            RefreshRequest request,
            RefreshUseCase useCase,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.Unauthorized();

            var result = await useCase.ExecuteAsync(request.RefreshToken, ct);
            if (!result.IsSuccess)
                return Results.Unauthorized();

            return Results.Ok(result.Value);
        })
        .WithSummary("Renova o Access Token")
        .WithDescription(
            "Gera um novo Access Token a partir de um Refresh Token válido. " +
            "O Refresh Token utilizado é revogado e substituído por um novo (rotação de token). " +
            "Retorna 401 caso o token esteja inválido, expirado ou revogado."
        );
    }
}