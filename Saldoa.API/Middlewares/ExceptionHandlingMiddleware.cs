using Microsoft.AspNetCore.Mvc;
using Saldoa.Application.Common.Exceptions;
using Saldoa.Domain.Exceptions;

namespace Saldoa.API.Middlewares;

internal class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    private bool IsDevelopment => _env.IsDevelopment();

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                ex,
                "Violação de regra de domínio. RequestId: {RequestId}, {Method} {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);

            await WriteProblemAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    title: "Domain.ValidationError",
                    detail: ex.Message
            );
        }
        catch (PersistenceConflictException ex)
        {
            _logger.LogWarning(
                ex,
                "Conflito ao persistir dados. RequestId: {RequestId}, {Method} {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);

            await WriteProblemAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    title: "Persistence.Conflict",
                    detail: ex.Message
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado. RequestId: {RequestId}, {Method} {Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);

            await WriteProblemAsync(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Server.UnexpectedError",
                detail: IsDevelopment ? ex.Message : "Erro interno no servidor"
            );
        }
    }

    private static async Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();

        await TypedResults.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: context.Request.Path
        ).ExecuteAsync(context);
    }
}
