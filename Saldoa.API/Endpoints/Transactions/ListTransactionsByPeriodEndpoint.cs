using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListByPeriod;

namespace Saldoa.API.Endpoints.Transactions;

public static class ListTransactionsByPeriodEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            [AsParameters] ListTransactionsByPeriodRequest request,
            IValidator<ListTransactionsByPeriodRequest> validator,
            ListTransactionsByPeriodUseCase useCase,
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
        })
        .WithSummary("Obtém lista de transações de um período")
        .WithDescription("Obtém lista de transações de um período com paginação e podendo filtrar pelo tipo (despesa ou renda). " + 
                         "O Período é opcional, caso não seja informado será retornado transações em um range da data atual até um ano atrás");
    }
}