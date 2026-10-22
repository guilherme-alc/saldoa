using Saldoa.Application.Common.Pagination;

namespace Saldoa.Application.Budgets.GetBudgetByCategory;

public sealed record GetBudgetsByCategoryRequest(
    int PageNumber = PaginationDefaults.PageNumber,
    int PageSize = PaginationDefaults.PageSize)
    : IPaginatedRequest;
