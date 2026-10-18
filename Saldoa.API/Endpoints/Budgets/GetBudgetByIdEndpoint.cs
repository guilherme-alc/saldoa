using Saldoa.API.Common;
using Saldoa.Application.Budgets.GetBudgetById;

namespace Saldoa.API.Endpoints.Budgets;

internal static class GetBudgetByIdEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapGet("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                GetBudgetByIdUseCase useCase,
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

                return TypedResults.Ok(result.Value);
            }
        )
        .RequireAuthorization("WorkspaceRead")
        .WithSummary("Obtém um orçamento")
        .WithDescription("Obtém um orçamento pelo Id");
    }
}