using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.Update;

namespace Saldoa.API.Endpoints.Transactions;

public class UpdateTransactionEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPatch("/{id:long}", async (
            long id,
            UpdateTransactionRequest request,
            UpdateTransactionUseCase useCase,
            IValidator<UpdateTransactionRequest> validator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(id, request, userId, ct);

            if (!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });

            return Results.NoContent();
        });
    }
}