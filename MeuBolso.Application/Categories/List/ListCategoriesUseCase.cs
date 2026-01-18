using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Categories.Common;
using MeuBolso.Application.Common.Pagination;
using MeuBolso.Application.Common.Results;

namespace MeuBolso.Application.Categories.List;

public class ListCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public ListCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<PagedResult<CategoryResponse>>> ExecuteAsync(string userId, int pageNumber, int pageSize, CancellationToken ct)
    {
        var data = await _categoryRepository.ListAsync(pageNumber, pageSize, userId, ct);
        
        var categoriesResponse = data.Items
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.Color)).ToList();

        var result = new PagedResult<CategoryResponse>(categoriesResponse, data.TotalCount, data.PageNumber, data.PageSize);
        
        return Result<PagedResult<CategoryResponse>>.Success(result);
    }
}