using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shared.Constants;
using Shared.Contracts;
using Shared.Enums;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Resources;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
        var responseCode = ResponseCodes.UnexpectedError;

        if (ex is BaseAppException bex)
        {
            responseCode = bex.ResponseCode;
        }

        var httpStatus = ResponseCodeMapper.GetHttpStatus(responseCode);
        var message = MessageResolver.GetResponseMessage(responseCode);

        _logger.LogError(ex,
            "GlobalException. ResponseCode: {ResponseCode}, Message: {Message}",
            responseCode, ex.Message);

        var apiResponse = new ApiResponse
        {
            Success = false,
            Status = httpStatus,
            Message = message,
            ResponseCode = responseCode,
            Data = null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = httpStatus;

        if (!context.Response.HasStarted)
        {
            await context.Response.WriteAsJsonAsync(apiResponse);
        }
    }
}