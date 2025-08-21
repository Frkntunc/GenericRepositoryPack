using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using ApplicationService.SharedKernel.Auth;
using WebAPI.Middleware;
using WebAPI.Filters;
using Microsoft.OpenApi.Models;
using ApplicationService.Extensions;
using Infrastructure.Extensions;
using Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using Domain.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "JWT Bearer token ile yetkilendirme için. Örn: Bearer {token}",

        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });
});


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CsrfOptions>(builder.Configuration.GetSection("CsrfOptions"));

builder.Services
    .AddControllers()
    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(WebAPI.Controllers.UserController).Assembly));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "Geçersiz istek.";

        throw new ApplicationService.SharedKernel.Exceptions.ValidationException(firstError, ErrorCodes.ValidationError);
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UnitOfWorkSaveChangesFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UnitOfWorkSaveChangesFilter>();
    options.Filters.Add<ResultWrappingFilter>();
});

var app = builder.Build();

// Veritabanı otomatik migrate
using (var scope = app.Services.CreateScope())
{
    var tracker = scope.ServiceProvider.GetRequiredService<MigrationTracker>();
    await tracker.ApplyMigrationsAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseMiddleware<CsrfMiddleware>();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        return Task.CompletedTask;
    });
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapControllers();
app.Run();
