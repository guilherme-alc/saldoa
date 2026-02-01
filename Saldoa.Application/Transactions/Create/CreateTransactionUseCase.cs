using Saldoa.Application.Categories.Abstractions;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Transactions.Abstractions;
using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Transactions.Create;

public class CreateTransactionUseCase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unit;
    
    public CreateTransactionUseCase(ITransactionRepository transactionRepository, IUnitOfWork unit, ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _unit = unit;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<TransactionResponse>> ExecuteAsync(CreateTransactionRequest request, string userId, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, userId, ct);
        
        if(category == null)
            return Result<TransactionResponse>.Failure("Categoria não encontrada");
        
        var transaction = new Transaction(
            userId, 
            request.Title, 
            request.Description, 
            request.Type, 
            request.Amount, 
            request.PaidOrReceivedAt, 
            category.Id);

        await _transactionRepository.AddAsync(transaction, ct);
        await _unit.SaveChangesAsync(ct);
        
        return Result<TransactionResponse>.Success(
            new TransactionResponse(
                transaction.Id, 
                transaction.Title, 
                transaction.Description, 
                transaction.Type, 
                transaction.Amount, 
                transaction.PaidOrReceivedAt, 
                new CategorySummaryResponse(category.Id, category.Name, category.Color))
        );
    }
}