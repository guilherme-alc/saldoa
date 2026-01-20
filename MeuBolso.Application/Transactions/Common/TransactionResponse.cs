using MeuBolso.Application.Categories.Common;
using MeuBolso.Domain.Enums;

namespace MeuBolso.Application.Transactions.Common;

public record CategorySummaryResponse(long Id, string Name, string? Color);

public record TransactionResponse(
    long Id, 
    string Title, 
    string? Description, 
    ETransactionType Type, 
    decimal Amount, 
    DateOnly? PaidOrReceivedAt,
    CategorySummaryResponse Category
);