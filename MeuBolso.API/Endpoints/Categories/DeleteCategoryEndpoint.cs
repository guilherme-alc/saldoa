using System.Security.Claims;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Categories.Delete;

namespace MeuBolso.API.Endpoints.Categories;

public static class DeleteCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long:min(1)}", async (
            long id,
            DeleteCategoryUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(id, userId, ct);
            
            if (!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });
            
            return Results.NoContent();
        });
    }
}