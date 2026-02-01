using System.Security.Claims;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.GetById;

namespace Saldoa.API.Endpoints.Categories;

public static class GetCategoryByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long:min(1)}", async (
            long id,
            GetCategoryByIdUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct
        ) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(id, userId, ct);
            
            if(!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });
            
            return Results.Ok(result.Value);
        });
    }
}