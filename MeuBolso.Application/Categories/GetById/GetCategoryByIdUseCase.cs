using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Categories.Common;
using MeuBolso.Application.Common.Results;

namespace MeuBolso.Application.Categories.GetById;

public class GetCategoryByIdUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId, ct);
        
        if (category is null)
            return Result<CategoryResponse>.Failure("Categoria não encontrada");
        
        var response = new CategoryResponse(category.Id, category.Name, category.Description, category.Color);
        
        return Result<CategoryResponse>.Success(response);
    }
}