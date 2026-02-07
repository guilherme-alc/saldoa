using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Create;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class CreateCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCategoryBudgetRequest request,
            IValidator<CreateCategoryBudgetRequest> validator,
            CreateCategoryBudgetUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if(!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, request, ct);
            
            if(!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });
            
            var response = result.Value!;
            
            return Results.Created(
                $"/category-budgets/{response.Id}",
                response);
        });
    }
}