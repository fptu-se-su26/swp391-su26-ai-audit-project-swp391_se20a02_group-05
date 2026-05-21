using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using CVerify.API.Application.Exceptions;
using CVerify.API.Infrastructure.Diagnostics;

namespace CVerify.API.API.Extensions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IAppLogger _logger;

    public GlobalExceptionHandler(IAppLogger logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server Error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = exception.Message
        };

        bool isHandled = false;

        if (exception is DuplicateEmailException dupEx)
        {
            isHandled = true;
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Title = "Duplicate Email Conflict";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
            problemDetails.Extensions.Add("code", dupEx.Code);
        }
        else if (exception is AuthException authEx)
        {
            isHandled = true;
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Authentication Error";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            problemDetails.Extensions.Add("code", authEx.Code);
        }
        else if (exception is UnauthorizedAccessException)
        {
            isHandled = true;
            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "Unauthorized";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
        }

        // Centralized Exception Log routing
        if (isHandled)
        {
            _logger.Log(LogLevel.Warning, "SYSTEM", $"Handled exception: {exception.GetType().Name} - {exception.Message}");
        }
        else
        {
            _logger.Log(LogLevel.Error, "SYSTEM", $"An unhandled exception occurred: {exception.Message}", exception);
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
