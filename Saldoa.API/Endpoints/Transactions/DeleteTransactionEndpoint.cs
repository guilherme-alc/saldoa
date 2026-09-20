using Saldoa.API.Common;
using Saldoa.Application.Transactions.Delete;

namespace Saldoa.API.Endpoints.Transactions;

internal static class DeleteTransactionEndpoint
{
    internal static void Map(RouteGroupBuilder transactionsGroup)
    {
        transactionsGroup.MapDelete("/{id:long}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                DeleteTransactionUseCase useCase,
                CancellationToken ct) =>
            {
                var result = await useCase.ExecuteAsync(id, workspaceId, ct);

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
        .WithSummary("Remove uma transação")
        .WithDescription("Remove transação pelo Id");
    }
}