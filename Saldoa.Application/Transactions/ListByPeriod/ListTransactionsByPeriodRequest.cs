using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.ListByPeriod;

public sealed record ListTransactionsByPeriodRequest(
    DateOnly? StartDate, 
    DateOnly? EndDate, 
    ETransactionType? Type, 
    int PageNumber = 1, 
    int PageSize = 20);