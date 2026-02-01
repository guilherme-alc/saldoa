using System.Security.Claims;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Transactions.Delete;

namespace MeuBolso.API.Endpoints.Transactions;

public class DeleteTransactionEndpoint
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