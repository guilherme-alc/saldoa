using Saldoa.API.Common;
using Saldoa.Application.Transactions.GetById;

namespace Saldoa.API.Endpoints.Transactions;

internal static class GetTransactionByIdEndpoint
{
    internal static void Map(RouteGroupBuilder transactionsGroup)
    {
        transactionsGroup.MapGet("/{id:long}", 
            async Task<IResult> (
                long id,
                GetTransactionByIdUseCase useCase,
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

                var response = result.Value!;

                return TypedResults.Ok(response);
            }
        )
        .RequireAuthorization("WorkspaceRead")
        .WithSummary("Obtém uma transação")
        .WithDescription("Obtém uma transação pelo Id");
    }
}