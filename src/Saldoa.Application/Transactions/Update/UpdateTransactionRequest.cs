using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Update;

public record UpdateTransactionRequest(
    string Title, 
    string? Description, 
    decimal Amount, 
    DateOnly PaidOrReceivedAt, 
    long CategoryId,
    TransactionUpdateScope? UpdateScope
);