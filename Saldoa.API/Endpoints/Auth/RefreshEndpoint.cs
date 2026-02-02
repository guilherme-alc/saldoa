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
        });
    }
}