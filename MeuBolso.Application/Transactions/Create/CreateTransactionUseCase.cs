using MeuBolso.Application.Categories.Abstractions;
using MeuBolso.Application.Common.Abstractions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Transactions.Abstractions;
using MeuBolso.Application.Transactions.Common;
using MeuBolso.Domain.Entities;

namespace MeuBolso.Application.Transactions.Create;

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