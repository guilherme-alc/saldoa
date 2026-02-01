using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.Update;

public record UpdateTransactionRequest(string? Title, string? Description, decimal? Amount, ETransactionType? Type, DateOnly? PaidOrReceivedAt, long? CategoryId);