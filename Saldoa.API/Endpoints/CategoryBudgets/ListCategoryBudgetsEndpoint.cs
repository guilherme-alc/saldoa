using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.ListCategoryBudgets;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class ListCategoryBudgetsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (            
            [AsParameters] ListCategoryBudgetsRequest request,
            IValidator<ListCategoryBudgetsRequest> validator,
            ListCategoryBudgetsUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, request, ct);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });
            
            return Results.Ok(result.Value);
        })
        .WithSummary("Obtém lista de limite de gasto por categoria")
        .WithDescription(
            "Obtém lista de limites de gastos com filtros opcionais de período e status. " +
            "O filtro 'active' considera a data atual (UTC) " +
            "true = vigente hoje; false = expirado ou futuro." +
            "Filtros podem ser combinados. "
        );
    }
}