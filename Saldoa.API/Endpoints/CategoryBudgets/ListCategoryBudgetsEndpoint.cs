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
        });
    }
}