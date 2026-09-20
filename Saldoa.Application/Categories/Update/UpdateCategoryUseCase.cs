using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.Update;

public class UpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public UpdateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
         _categoryRepository = categoryRepository;
         _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, UpdateCategoryRequest request, Guid workspaceId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdForUpdateAsync(id, workspaceId, ct);

        if (category == null)
        {
            var error = CategoryErrors.NotFound;
            return Result.Failure(error);
        }

        var nameNormalized = request.Name.Trim().ToUpperInvariant();
        if (nameNormalized != category.NormalizedName 
            && await _categoryRepository.ExistsAsync(workspaceId, request.Name, ct))
        {
            var error = CategoryErrors.AlreadyExists(request.Name);
            return Result.Failure(error);
        }
            
        category.Rename(request.Name);
        category.ChangeDescription(request.Description);
        category.ChangeColor(request.Color);
        
        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}