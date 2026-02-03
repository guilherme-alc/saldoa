using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListPending;

namespace Saldoa.API.Endpoints.Transactions;

public static class ListPendingTransactionsEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/pending", async (
            [AsParameters] ListPendingTransactionsRequest request,
            IValidator<ListPendingTransactionsRequest> validator,
            ListPendingTransactionsUseCase useCase,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();
            
            var result = await useCase.ExecuteAsync(userId, request, ct);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });
            
            return Results.Ok(result.Value);
        });
    }
}