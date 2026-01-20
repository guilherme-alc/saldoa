using System.Security.Claims;
using FluentValidation;
using MeuBolso.API.Extensions;
using MeuBolso.Application.Transactions.Create;

namespace MeuBolso.API.Endpoints.Transactions;

public class CreateTransactionEndpoint
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