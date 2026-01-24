using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Transactions.Abstractions;
using MeuBolso.Application.Transactions.Common;

namespace MeuBolso.Application.Transactions.Update;

public class UpdateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    
    public UpdateTransactionUseCase(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository, IUnitOfWork unit)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _unit = unit;
    }
    
    public async Task<Result> ExecuteAsync(
        long id, 
        UpdateTransactionRequest request,
        string userId, 
        CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetByIdForUpdateAsync(id, userId, ct);
        if (transaction is null)
            return Result.Failure("Transação não encontrada");
        
        if (request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, userId, ct);
            if (category is null)
                return Result.Failure("Categoria não encontrada");
        }
        
        transaction.SetTitle(request.Title);
        transaction.SetDescription(request.Description);
        transaction.SetAmount(request.Amount);
        transaction.SetPaidOrReceivedAt(request.PaidOrReceivedAt);
        transaction.SetCategoryId(request.CategoryId);

        await _unit.SaveChangesAsync(ct);
        return Result.Success();
    }
}