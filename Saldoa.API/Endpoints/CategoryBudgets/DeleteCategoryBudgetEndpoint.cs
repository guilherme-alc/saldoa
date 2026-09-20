using Saldoa.API.Common;
using Saldoa.Application.CategoryBudgets.Delete;

namespace Saldoa.API.Endpoints.CategoryBudgets;

internal static class DeleteCategoryBudgetEndpoint
{
    internal static void Map(RouteGroupBuilder categoryBudgetsGroup)
    {
        categoryBudgetsGroup.MapDelete("/{id:long:min(1)}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                DeleteCategoryBudgetUseCase useCase,
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

                return TypedResults.NoContent();
            }
        )
        .WithSummary("Remove um limite de gasto por categoria")
        .WithDescription("Remove um limite de gasto pelo Id");
    }
}