using FluentValidation;
using Saldoa.API.Common;
using Saldoa.Application.Categories.Create;

namespace Saldoa.API.Endpoints.Categories;

internal static class CreateCategoryEndpoint
{
    internal static void Map(RouteGroupBuilder categoriesGroup)
    {
        categoriesGroup.MapPost("/", 
            async Task<IResult>(
                Guid workspaceId,
                CreateCategoryRequest request,
                CreateCategoryUseCase useCase,
                IValidator<CreateCategoryRequest> validator,
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

                var result = await useCase.ExecuteAsync(request, workspaceId, ct);
            
                if (!result.IsSuccess)
                {
                    var error = result.Error!;
                    int statusCode = StatusCodeMapper.GetCode(error.Type);

                    return TypedResults.Problem(
                        detail: error.Message,
                        statusCode: statusCode,
                        title: error.Code
                    );
                }
            
                var response = result.Value!;
            
                return TypedResults.Created($"/categories/{response.Id}", response);
            }
        )
        .WithSummary("Cria nova categoria")
        .WithDescription("Cria nova categoria Nome (obrigatório), descrição e cor (opcionais)");
    }
}