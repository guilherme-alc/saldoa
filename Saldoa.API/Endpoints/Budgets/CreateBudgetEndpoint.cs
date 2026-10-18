using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Budgets.Create;

namespace Saldoa.API.Endpoints.Budgets;

internal static class CreateBudgetEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapPost("/", 
            async Task<IResult> (
                CreateBudgetRequest request,
                IValidator<CreateBudgetRequest> validator,
                CreateBudgetUseCase useCase,
                CancellationToken ct) =>
            {
                var validation = await validator.ValidateAsync(request, ct);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                    return TypedResults.ValidationProblem(
                        errors: errors,
                        detail: "Um ou mais campos possuem erros de validação.",
                        title: "Entrada inválida"
                    );
                }

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
            
                var response = result.Value!;
            
                return TypedResults.Created($"/budgets/{response.Id}", response);
            }
        )
        .RequireAuthorization("WorkspaceWrite")
        .WithSummary("Cria um orçamento por categoria ou geral para um período específico")
        .WithDescription("Cria orçamento informando: Caso seja um orçamento por categoria será necessário informar o Id da categoria e o scope sendo 'category'; " +
        "caso seja um orçamento geral o scope é 'general' e sem id de categoria; " +
        "périodo em que essa regra será aplicada e o limite definido.");
    }
}