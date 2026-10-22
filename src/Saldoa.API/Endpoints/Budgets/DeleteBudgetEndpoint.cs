using Saldoa.API.Common;
using Saldoa.Application.Budgets.Delete;

namespace Saldoa.API.Endpoints.Budgets;

internal static class DeleteBudgetEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapDelete("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                DeleteBudgetUseCase useCase,
                CancellationToken ct) =>
            {      
                var result = await useCase.ExecuteAsync(id, ct);

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
        .RequireAuthorization("WorkspaceWrite")
        .WithSummary("Remove um orçamento")
        .WithDescription("Remove um orçamento informando Id");
    }
}