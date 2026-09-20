using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Transactions.Update;

namespace Saldoa.API.Endpoints.Transactions;

internal static class UpdateTransactionEndpoint
{
    internal static void Map(RouteGroupBuilder transactionsGroup)
    {
        transactionsGroup.MapPut("/{id:long}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                UpdateTransactionRequest request,
                UpdateTransactionUseCase useCase,
                IValidator<UpdateTransactionRequest> validator,
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

                var result = await useCase.ExecuteAsync(id, request, workspaceId, ct);

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
        .WithSummary("Atualiza uma transação")
        .WithDescription("Atualiza uma transação.");
    }
}
