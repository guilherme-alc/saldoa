using MeuBolso.Application.Common.Security;

namespace MeuBolso.Application.Categories.Create;

public sealed record CreateCategoryRequest ( string Name, string? Description, string? Color);