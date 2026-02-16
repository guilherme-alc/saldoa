using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.CategoryBudgets.Update;

namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class UpdateCategoryBudgetEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:long:min(1)}", async (
            long id,
            UpdateCategoryBudgetRequest request,
            UpdateCategoryBudgetUseCase useCase,
            IValidator<UpdateCategoryBudgetRequest> validator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if(!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, id, request, ct);
            
            if(!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });
            
            return Results.NoContent();
        })
        .WithSummary("Atualiza limite de gasto da categoria")
        .WithDescription(
            "Atualiza parcialmente o limite. Apenas propriedades enviadas com valor diferente de null serão alteradas. " +
            "Não será possível atualizar limites já expirados, ou que a atualização crie sobreposição de limites já existentes"
        );
    }
}