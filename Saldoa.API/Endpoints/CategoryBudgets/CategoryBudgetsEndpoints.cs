namespace Saldoa.API.Endpoints.CategoryBudgets;

public static class CategoryBudgetsEndpoints
{
    public static void MapCategoryBudgetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/category-budgets")
            .WithTags("Category Budgets");

        CreateCategoryBudgetEndpoint.Map(group);
        ListCategoryBudgetsEndpoint.Map(group);
        DeleteCategoryBudgetEndpoint.Map(group);
        GetCategoryBudgetByIdEndpoint.Map(group);
    }
}