using Saldoa.API.Common;
using Saldoa.Application.Categories.Delete;

namespace Saldoa.API.Endpoints.Categories;

internal static class DeleteCategoryEndpoint
{
    internal static void Map(RouteGroupBuilder categoriesGroup)
    {
        categoriesGroup.MapDelete("/{id:long:min(1)}", 
            async Task<IResult> (
                Guid workspaceId,
                long id,
                DeleteCategoryUseCase useCase,
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
            
                return TypedResults.NoContent();
            }
        )
        .WithSummary("Remove uma categoria")
        .WithDescription("Remove categoria pelo Id");
    }
}