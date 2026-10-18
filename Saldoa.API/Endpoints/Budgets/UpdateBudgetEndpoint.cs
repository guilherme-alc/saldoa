using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Budgets.Update;

namespace Saldoa.API.Endpoints.Budgets;

internal static class UpdateBudgetEndpoint
{
    internal static void Map(RouteGroupBuilder budgetsGroup)
    {
        budgetsGroup.MapPut("/{id:long:min(1)}", 
            async Task<IResult> (
                long id,
                UpdateBudgetRequest request,
                UpdateBudgetUseCase useCase,
                IValidator<UpdateBudgetRequest> validator,
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
            
                var result = await useCase.ExecuteAsync(id, request, ct);

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

                return TypedResults.NoContent();
            }
        )
        .RequireAuthorization("WorkspaceWrite")
        .WithSummary("Atualiza um orçamento")
        .WithDescription(
            "Atualiza o orçamento. " +
            "Não sendo possível atualizar orçamentos já expirados, ou que a atualização crie sobreposição de limites já existentes"
        );
    }
}