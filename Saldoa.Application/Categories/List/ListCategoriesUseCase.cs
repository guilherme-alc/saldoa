using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Pagination;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.List;

public class ListCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<CategoryResponse>> ExecuteAsync(Guid workspaceId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var data = await _categoryRepository.ListAsync(pageNumber, pageSize, workspaceId, ct);
        
        var categoriesResponse = data.Items
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.Color)).ToList();

        var result = new PagedResult<CategoryResponse>(categoriesResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return result;
    }
}