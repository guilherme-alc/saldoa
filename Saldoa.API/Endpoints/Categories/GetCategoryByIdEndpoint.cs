using Saldoa.API.Common;
using Saldoa.Application.Categories.GetById;

namespace Saldoa.API.Endpoints.Categories;

internal static class GetCategoryByIdEndpoint
{
    internal static void Map(RouteGroupBuilder categoriesGroup)
    {
        categoriesGroup.MapGet("/{id:long:min(1)}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                GetCategoryByIdUseCase useCase,
                CancellationToken ct) =>
            {            
                var result = await useCase.ExecuteAsync(id, workspaceId, ct);
            
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
            
                return TypedResults.Ok(result.Value);
            }
        )
        .WithSummary("Obtém uma categoria")
        .WithDescription("Obtém uma categoria pelo Id");
    }
}