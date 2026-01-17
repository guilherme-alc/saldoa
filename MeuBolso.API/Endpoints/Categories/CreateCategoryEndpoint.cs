using System.Security.Claims;
using FluentValidation;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Categories.Create;

namespace MeuBolso.API.Endpoints.Categories;

public static class CreateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateCategoryRequest request,
            CreateCategoryUseCase useCase,
            IValidator<CreateCategoryRequest> validator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(request, userId, ct);
            
            if (!result.IsSuccess)
                return Results.Conflict(new { error = result.Error });
            
            var response = result.Value!;
            
            return Results.Created(
                $"/categories/{response.id}",
                response);
        });
    }
}