using System.Security.Claims;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.Delete;

namespace Saldoa.API.Endpoints.Transactions;

public static class DeleteTransactionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long}", async (
            long id,
            DeleteTransactionUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(id, userId, ct);
            
            if(!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });

            return Results.NoContent();
        });
    }
}