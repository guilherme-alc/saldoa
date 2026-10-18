using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Budgets.GetBudgetByCategory;

namespace Saldoa.API.Endpoints.Budgets;

internal static class GetBudgetsByCategoryEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapGet("/by-category/{categoryId:long:min(1)}", 
            async Task<IResult> (
                long categoryId,
                [AsParameters] GetBudgetsByCategoryRequest request,
                IValidator<GetBudgetsByCategoryRequest> validator,
                GetBudgetsByCategoryUseCase useCase,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if(!validation.IsValid)
                    return Results.BadRequest(validation.Errors);

                var result = await useCase.ExecuteAsync(categoryId, request, ct);

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
        .RequireAuthorization("WorkspaceRead")
        .WithSummary("Obtém lista de orçamentos")
        .WithDescription("Obtém lista de orçamentos de uma categoria especificada na rota");
    }
}