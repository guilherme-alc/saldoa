using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListByMonth;

namespace Saldoa.API.Endpoints.Transactions;

public static  class ListTransactionsByMonthEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/by-month", async (
            [AsParameters] ListTransactionsByMonthRequest request,
            IValidator<ListTransactionsByMonthRequest> validator,
            ListTransactionsByMonthUseCase useCase,
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