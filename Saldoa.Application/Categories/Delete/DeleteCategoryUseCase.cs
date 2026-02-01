using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;

namespace Saldoa.Application.Categories.Delete;

public class DeleteCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;

    public DeleteCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
        _categoryRepository = categoryRepository;
        _unit = unit;
    }

    public async Task<Result> ExecuteAsync(long id, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdForUpdateAsync(id, userId, ct);
        
        if (category is null)
            return Result.Failure("Categoria não encontrada");
        
        if (await _categoryRepository.HasTransactionsAsync(id, userId, ct))
            return Result.Failure("Não é possível excluir uma categoria com transações");
        
        _categoryRepository.Remove(category);
        await _unit.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}