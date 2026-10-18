namespace Saldoa.API.Endpoints.Budgets;

internal static class BudgetsEndpoints
{
    internal static void MapBudgetEndpoints(this IEndpointRouteBuilder app)
    {
        var budgetsGroup = app.MapGroup("/budgets")
            .WithTags("Budgets");

        CreateBudgetEndpoint.Map(budgetsGroup);
        ListBudgetsEndpoint.Map(budgetsGroup);
        DeleteBudgetEndpoint.Map(budgetsGroup);
        GetBudgetByIdEndpoint.Map(budgetsGroup);
        UpdateBudgetEndpoint.Map(budgetsGroup);
        GetBudgetsByCategoryEndpoint.Map(budgetsGroup);
    }
}