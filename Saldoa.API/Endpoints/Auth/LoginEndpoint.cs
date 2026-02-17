using FluentValidation;
using Saldoa.Application.Auth.Login;

namespace Saldoa.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", async (
            LoginRequest request,
            IValidator<LoginRequest> validator,
            LoginUseCase useCase,
            CancellationToken ct) =>
        {
            var validation = await validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);
            
            var result = await useCase.ExecuteAsync(request, ct);
            
            if (!result.IsSuccess)
                return Results.Json(new { message = "Acesso inválido" }, statusCode: 401);
            
            return Results.Ok(result.Value);
        })
        .WithSummary("Autentica o usuário")
        .WithDescription(
            "Realiza a autenticação utilizando e-mail e senha. " +
            "Em caso de sucesso, retorna um Access Token (JWT) e um Refresh Token com suas respectivas datas de expiração. " +
            "Em caso de credenciais inválidas, retorna 401."
        );
    }
}