using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Categories.Update;

namespace Saldoa.API.Endpoints.Categories;

public static class UpdateCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:long}", async (
            long id,
            UpdateCategoryRequest request,
            UpdateCategoryUseCase useCase,
            IValidator<UpdateCategoryRequest> validator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(id, request, userId, ct);
            
            if (!result.IsSuccess)
                return Results.Conflict(new { error = result.Error });
            
            return Results.NoContent();
        });
    }
}