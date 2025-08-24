using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Resources;
using System.Net;
using System.Text.Json;
using WebAPI.DTOs;

namespace WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IStringLocalizer<ErrorCodeResources> _localizer;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IStringLocalizer<ErrorCodeResources> localizer)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, errorCode) = ex switch
        {
            NotFoundException nfEx => (StatusCodes.Status404NotFound, nfEx.ErrorCode),
            ValidationException valEx => (StatusCodes.Status400BadRequest, valEx.ErrorCode),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.UnexpectedError)
        };

        var message = _localizer[errorCode.ToString()];
        var problem = new ProblemDetails
        {
            Title = message,
            Detail = ex.Message,
            Status = statusCode,
            Type = $"https://httpstatuses.com/{statusCode}",
            Extensions =
            {
                ["code"] = errorCode.ToString()
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        _logger.LogError(ex, "ErrorCode: {ErrorCode}, Message: {Message}", errorCode, message);

        await context.Response.WriteAsJsonAsync(problem);
    }
}