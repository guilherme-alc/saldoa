using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class GetCategoryBudgetsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/categories/{categoryId:long:min(1)}/budgets", async (
            long categoryId,
            [AsParameters] GetCategoryBudgetsByCategoryRequest request,
            IValidator<GetCategoryBudgetsByCategoryRequest> validator,
            GetCategoryBudgetsByCategoryUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if(!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(userId, categoryId, request, ct);
            
            if(!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error } );
            
            return Results.Ok(result.Value);
        })
        .WithSummary("Obtém lista de limite de gasto de uma categoria")
        .WithDescription(
            "Obtém lista de limites de gastos da categoria especificada na rota"
        );
    }
}