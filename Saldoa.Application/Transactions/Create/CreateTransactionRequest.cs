using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Create;

public sealed record CreateTransactionRequest(string Title, string? Description, DateOnly? PaidOrReceivedAt, ETransactionType Type, decimal Amount, long CategoryId);