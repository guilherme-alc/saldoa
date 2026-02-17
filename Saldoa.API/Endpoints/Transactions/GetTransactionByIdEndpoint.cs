using System.Security.Claims;
using Saldoa.API.Extensions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.GetById;

namespace Saldoa.API.Endpoints.Transactions;

public static class GetTransactionByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", async (
                long id,
                GetTransactionByIdUseCase useCase,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var userId = user.GetUserId();

                var result = await useCase.ExecuteAsync(id, userId, ct);

                if (!result.IsSuccess)
                    return Results.NotFound(new { error = result.Error });

                var response = result.Value!;

                return Results.Ok(response);
            })
            .WithSummary("Obtém uma transação")
            .WithDescription("Obtém uma transação pelo Id");
    }
}