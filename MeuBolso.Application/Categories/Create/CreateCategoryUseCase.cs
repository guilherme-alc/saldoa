using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Categories.Exceptions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Categories.Create;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryUseCase(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCategoryResponse> ExecuteAsync(CreateCategoryRequest request)
    {
        if (await _categoryRepository.ExistsAsync(request.UserId, request.Name))
            throw new CategoryAlreadyExistsException(request.Name);

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Color = request.Color,
            UserId = request.UserId
        };
    
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        
        return new CreateCategoryResponse(category.Id, category.Name);
    }
}