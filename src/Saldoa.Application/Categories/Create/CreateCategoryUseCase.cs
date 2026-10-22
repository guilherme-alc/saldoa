using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Categories.Common;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Categories.Create;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    private readonly IWorkspaceContext _workspaceContext;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit, IWorkspaceContext workspaceContext)
    {
        _categoryRepository = categoryRepository;
        _unit = unit;
        _workspaceContext = workspaceContext;
    }

    public async Task<Result<CreateCategoryResponse>> ExecuteAsync(CreateCategoryRequest request, CancellationToken ct)
    {
        if (await _categoryRepository.ExistsAsync(request.Name, ct))
        {
            var error = CategoryErrors.AlreadyExists(request.Name);
            return Result<CreateCategoryResponse>.Failure(error);
        }

        var category = new Category(_workspaceContext.WorkspaceId!.Value, request.Name, request.Description, request.Color);
    
        await _categoryRepository.AddAsync(category, ct);
        await _unit.SaveChangesAsync(ct);
        return Result<CreateCategoryResponse>.Success(
            new CreateCategoryResponse(category.Id, category.Name)
        );
    }
}