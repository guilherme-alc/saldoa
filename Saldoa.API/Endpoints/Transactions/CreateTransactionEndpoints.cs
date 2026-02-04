using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.Create;

namespace Saldoa.API.Endpoints.Transactions;

public static class CreateTransactionEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateTransactionRequest request,
            CreateTransactionUseCase useCase,
            IValidator<CreateTransactionRequest> validator,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var userId = user.GetUserId();

            var result = await useCase.ExecuteAsync(request, userId, ct);
            
            if (!result.IsSuccess)
                return Results.BadRequest(new { error = result.Error });
            
            var response = result.Value!;
            
            return Results.Created(
                $"/transactions/{response.Id}",
                response);
        });
    }
}