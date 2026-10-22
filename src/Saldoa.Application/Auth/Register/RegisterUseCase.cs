using Microsoft.Extensions.Logging;
using Saldoa.Application.Auth.Common;
using Saldoa.Application.Auth.ConfirmEmail;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Application.Common.Results;
using Saldoa.Application.Email;
using Saldoa.Application.Identity;
using Saldoa.Application.Identity.Abstractions;
using Saldoa.Application.Workspaces.Abstractions;
using Saldoa.Domain.Entities;

namespace Saldoa.Application.Auth.Register;

public class RegisterUseCase
{
    private readonly IIdentityService _identityService;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUseCase> _logger;

    public RegisterUseCase(
        IIdentityService identityService,
        IBackgroundJobService backgroundJobService,
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUseCase> logger)
    {
        _identityService = identityService;
        _backgroundJobService = backgroundJobService;
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(RegisterRequest request, CancellationToken ct)
    {
        Result<CreateUserResult> result;
        try
        {
            if (await _identityService.UserExistsAsync(request.Email, ct))
            {
                var error = AuthErrors.AlreadyExists;
                return Result.Failure(error);
            }

            await _unitOfWork.BeginTransactionAsync(ct);

            result = await _identityService.CreateUserAsync(
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                ct);

            if (!result.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                return Result.Failure(result.Error!);
            }

            var workspace = new Workspace("Personal", result.Value!.UserId);
            await _workspaceRepository.AddAsync(workspace, ct);

            await _unitOfWork.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            try
            {
                await _unitOfWork.RollbackTransactionAsync(
                    CancellationToken.None);
            }
            catch (Exception rollbackException)
            {
                _logger.LogError(
                    rollbackException,
                    "Falha ao executar rollback durante o cadastro de usuário.");
            }

            throw;
        }

        var body = $"""
            <h1>Confirme seu e-mail</h1>
            <p>
                Seu Id é: {result.Value!.UserId}
            </p>
            <p>
                Seu token é: {result.Value.ConfirmationToken}
            </p>
            """;

        EmailMessage emailMessage = new(result.Value.Email, "Bem-vindo ao Saldoa! - Confirme seu e-mail", body);

        _backgroundJobService.Enqueue<SendEmailConfirmationJob>(x => x.ExecuteAsync(emailMessage, result.Value.UserId, CancellationToken.None));

        return Result.Success();
    }
}
