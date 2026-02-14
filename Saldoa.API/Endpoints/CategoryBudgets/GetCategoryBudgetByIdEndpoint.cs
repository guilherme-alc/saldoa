using System.Security.Claims;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class GetCategoryBudgetByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long:min(1)}", async (
            long id,
            GetCategoryBudgetByIdUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct
        ) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, id, ct);
            
            if(!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });
            
            return Results.Ok(result.Value);
        })
        .WithSummary("Obtém um limite de gasto por categoria")
        .WithDescription("Obtém um limite de gasto pelo Id e pelo Id do usuário autenticado");
    }
}