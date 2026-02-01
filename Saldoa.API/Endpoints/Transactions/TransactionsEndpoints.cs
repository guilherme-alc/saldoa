namespace Saldoa.API.Endpoints.Transactions;

public static class TransactionsEndpoints
{
    public static void MapTransactionsEndpoint(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions")
            .RequireAuthorization()
            .WithTags("Transactions");

        CreateTransactionEndpoint.Map(group);
        GetTransactionByIdEndpoint.Map(group);
        UpdateTransactionEndpoint.Map(group);
        DeleteTransactionEndpoint.Map(group);
    }
}