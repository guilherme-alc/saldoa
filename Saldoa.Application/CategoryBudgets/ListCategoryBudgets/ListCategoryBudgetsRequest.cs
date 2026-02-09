namespace Saldoa.Application.CategoryBudgets.ListCategoryBudgets;

public sealed record ListCategoryBudgetsRequest(   
    DateOnly? StartDate, 
    DateOnly? EndDate,
    bool? Active,
    int PageNumber = 1, 
    int PageSize = 20);