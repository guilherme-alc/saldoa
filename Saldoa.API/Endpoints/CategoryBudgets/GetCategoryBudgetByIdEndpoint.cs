using Saldoa.API.Common;
using Saldoa.Application.CategoryBudgets.GetCategoryBudgetById;

namespace Saldoa.API.Endpoints.CategoryBudgets;

internal static class GetCategoryBudgetByIdEndpoint
{
    internal static void Map(RouteGroupBuilder categoryBudgetsGroup)
    {
        categoryBudgetsGroup.MapGet("/{id:long:min(1)}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                GetCategoryBudgetByIdUseCase useCase,
                CancellationToken ct) =>
            {
                var result = await useCase.ExecuteAsync(workspaceId, id, ct);

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
        .WithSummary("Obtém um limite de gasto por categoria")
        .WithDescription("Obtém um limite de gasto pelo Id");
    }
}