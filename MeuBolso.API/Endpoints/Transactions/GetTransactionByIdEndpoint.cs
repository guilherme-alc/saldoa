using System.Security.Claims;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Transactions.GetById;

namespace MeuBolso.API.Endpoints.Transactions;

public class GetTransactionByIdEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", async(
            long id,
            GetTransactionByIdUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(id, userId, ct);
            
            if(!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });
            
            var response = result.Value!;
            
            return Results.Ok(response);
        });
    }
}