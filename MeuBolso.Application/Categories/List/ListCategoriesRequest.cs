namespace MeuBolso.Application.Categories.List;

public sealed record ListCategoriesRequest(int PageNumber = 1, int PageSize = 20);