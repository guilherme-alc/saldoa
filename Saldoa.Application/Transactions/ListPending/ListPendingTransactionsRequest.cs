using Saldoa.Application.Transactions.Common;
using Saldoa.Domain.Enums;

namespace Saldoa.Application.Transactions.ListPending;

public sealed record ListPendingTransactionsRequest(ETransactionType? Type, int PageNumber = 1, int PageSize = 20);