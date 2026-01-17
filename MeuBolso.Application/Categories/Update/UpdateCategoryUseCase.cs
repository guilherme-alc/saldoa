using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Common.Results;

namespace MeuBolso.Application.Categories.Update;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
         _categoryRepository = categoryRepository;
         _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, UpdateCategoryRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdForUpdateAsync(id, userId, ct);

        if (category == null)
            return Result.Failure("Categoria não encontrada");

        if (request.Name is not null)
        {
            var newNormalized = request.Name.Trim().ToUpperInvariant();
            if (newNormalized != category.NormalizedName 
                && await _categoryRepository.ExistsAsync(userId, request.Name, ct))
                return Result.Failure($"Uma Categoria com o nome {request.Name} já existe");
            
            category.SetName(request.Name);
        }

        if(request.Description is not null)
            category.SetDescription(request.Description);
        
        if(request.Color is not null)
            category.SetColor(request.Color);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}