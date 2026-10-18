using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Budgets.ListBudgets;

namespace Saldoa.API.Endpoints.Budgets;

internal static class ListBudgetsEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapGet("/", 
            async Task<IResult> (
                [AsParameters] ListBudgetsRequest request,
                IValidator<ListBudgetsRequest> validator,
                ListBudgetsUseCase useCase,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid)
                    return Results.BadRequest(validation.Errors);
            
                var result = await useCase.ExecuteAsync(request, ct);

                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    var statusCode = StatusCodeMapper.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }

                return TypedResults.Ok(result.Value);
            }
        )
        .RequireAuthorization("WorkspaceRead")
        .WithSummary("Obtém lista de orçamentos")
        .WithDescription(
            "Obtém lista de orçamentos com filtros opcionais de período e status. " +
            "O filtro 'active' considera a data atual (UTC) " +
            "true = vigente hoje; false = expirado ou futuro." +
            "Filtros podem ser combinados. "
        );
    }
}