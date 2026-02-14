using System.Security.Claims;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Delete;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class DeleteCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long:min(1)}", async (
            long id,
            DeleteCategoryBudgetUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, id, ct);
            
            if (!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });
            
            return Results.NoContent();
        });
    }
}