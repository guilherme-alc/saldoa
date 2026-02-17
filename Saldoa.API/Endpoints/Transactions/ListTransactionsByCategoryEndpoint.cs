using System.Security.Claims;
using FluentValidation;
using Saldoa.API.Extensions;
using Saldoa.Application.Transactions.ListByCategory;

namespace Saldoa.API.Endpoints.Transactions;

public static class ListTransactionsByCategoryEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/by-category", async (
            [AsParameters] ListTransactionsByCategoryRequest request,
            IValidator<ListTransactionsByCategoryRequest> validator,
            ListTransactionsByCategoryUseCase useCase,
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
        .WithSummary("Obtém lista de transações de uma categoria")
        .WithDescription("Obtém lista de transações de uma categoria específica com paginação e podendo ser filtrada por período");
    }
}