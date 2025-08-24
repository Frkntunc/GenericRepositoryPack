using ApplicationService.Extensions;
using ApplicationService.SharedKernel.Auth;
using Domain.Extensions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using Persistence.Contracts;
using Serilog;
using Shared.Enums;
using Shared.Exceptions;
using System.Globalization;
using WebAPI.Filters;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// Logging (Serilog)
// -------------------------------------------------
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// -------------------------------------------------
// Services
// -------------------------------------------------
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
builder.Services.AddLocalization();

// Katman bağımlılıkları
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

// Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "JWT Bearer token kullanın. Örn: Bearer {token}",
        Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
    };

    opt.AddSecurityDefinition("Bearer", jwtScheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
});

// Cookie ayarları
builder.Services.Configure<CookiePolicyOptions>(opt =>
{
    opt.Secure = CookieSecurePolicy.Always;
    opt.MinimumSameSitePolicy = SameSiteMode.Strict;
    opt.HttpOnly = HttpOnlyPolicy.Always;
});

// Config binding
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CsrfOptions>(builder.Configuration.GetSection("CsrfOptions"));

// Controllers + Filters
builder.Services
    .AddControllers(o =>
    {
        o.Filters.Add<UnitOfWorkSaveChangesFilter>();
        o.Filters.Add<ResultWrappingFilter>();
    })
    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(WebAPI.Controllers.UserController).Assembly));

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = context =>
    {
        throw new ValidationException(ErrorCodes.ValidationError);
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UnitOfWorkSaveChangesFilter>();

var app = builder.Build();

// -------------------------------------------------
// Middleware pipeline
// -------------------------------------------------

var supportedCultures = new[] { "en", "tr" };
app.UseRequestLocalization(o =>
{
    o.SetDefaultCulture("en");
    o.AddSupportedCultures(supportedCultures);
    o.AddSupportedUICultures(supportedCultures);
});

// Database migration
using (var scope = app.Services.CreateScope())
{
    var tracker = scope.ServiceProvider.GetRequiredService<MigrationTracker>();
    await tracker.ApplyMigrationsAsync();
}

// Exception & Security Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseMiddleware<CsrfMiddleware>();

// Gereksiz headerları kaldır
app.Use(async (ctx, next) =>
{
    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers.Remove("Server");
        ctx.Response.Headers.Remove("X-Powered-By");
        return Task.CompletedTask;
    });
    await next();
});

// Swagger (sadece dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Default redirect
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();
