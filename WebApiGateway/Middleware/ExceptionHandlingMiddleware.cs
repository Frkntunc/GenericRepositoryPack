using System.Net;
using System.Text.Json;

namespace WebApiGateway.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu");

            context.Response.ContentType = "application/json";

            int statusCode;
            object problem;

            if (ex is ApplicationService.SharedKernel.Exceptions.NotFoundException nfEx)
            {
                statusCode = StatusCodes.Status404NotFound;
                problem = new
                {
                    title = "Bulunamadı",
                    detail = nfEx.Message,
                    code = (int)nfEx.Code,
                    status = 404
                };
            }
            else if (ex is ApplicationService.SharedKernel.Exceptions.ValidationException valEx)
            {
                statusCode = StatusCodes.Status400BadRequest;
                problem = new
                {
                    title = "Doğrulama hatası",
                    message = valEx.MessageText,
                    code = (int)valEx.Code,
                    status = 400
                };
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                problem = new
                {
                    title = "Bir hata oluştu",
                    detail = ex.Message,
                    status = 500
                };
            }

            context.Response.StatusCode = statusCode;
            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }

}
