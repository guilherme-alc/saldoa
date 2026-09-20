using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetByCategory;

namespace Saldoa.API.Endpoints.CategoryBudgets;

internal static class GetCategoryBudgetsByCategoryEndpoint
{
    internal static void Map(RouteGroupBuilder categoryBudgetsGroup)
    {
        categoryBudgetsGroup.MapGet("/by-category/{categoryId:long:min(1)}", 
            async Task<IResult> (
                Guid workspaceId,
                long categoryId,
                [AsParameters] GetCategoryBudgetsByCategoryRequest request,
                IValidator<GetCategoryBudgetsByCategoryRequest> validator,
                GetCategoryBudgetsByCategoryUseCase useCase,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if(!validation.IsValid)
                    return Results.BadRequest(validation.Errors);

                var result = await useCase.ExecuteAsync(workspaceId, categoryId, request, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    var statusCode = StatusCodeMapper.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém lista de limite de gasto de uma categoria")
        .WithDescription("Obtém lista de limites de gastos da categoria especificada na rota");
    }
}