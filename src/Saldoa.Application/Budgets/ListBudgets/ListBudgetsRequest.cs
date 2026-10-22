using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Budgets.ListBudgets;

public sealed record ListBudgetsRequest(
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool? Active,
    int PageNumber = PaginationDefaults.PageNumber,
    int PageSize = PaginationDefaults.PageSize)
    : IPaginatedRequest;
