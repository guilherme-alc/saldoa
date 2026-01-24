using MeuBolso.Domain.Enums;

namespace MeuBolso.Application.Transactions.Update;

public record UpdateTransactionRequest(string? Title, string? Description, decimal? Amount, ETransactionType? Type, DateOnly? PaidOrReceivedAt, long? CategoryId);