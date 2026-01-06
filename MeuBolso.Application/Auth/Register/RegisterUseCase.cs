using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Application.Common.Results;
using MeuBolso.Application.Identity.Abstractions;

namespace MeuBolso.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IJwtProvider _jwtProvider;

    public RegisterUseCase(IIdentityService identityService, IJwtProvider jwtProvider)
    {
        _identityService = identityService;
        _jwtProvider = jwtProvider;
    }
    public async Task<Result<string>> ExecuteAsync(RegisterRequest request)
    {
        if (await _identityService.UserExistsAsync(request.Email))
            return Result<string>.Failure("Usuário já existe");
        
        var userId = await _identityService.CreateUserAsync(
            request.Email, 
            request.Password, 
            request.FullName);

        return Result<string>.Success(userId);
    }
}