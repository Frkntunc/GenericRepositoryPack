using ApplicationService.Extensions;
using Domain.Extensions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Persistence.Contracts;
using Serilog;
using Serilog.Events;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Options;
using StackExchange.Redis;
using System.Globalization;
using WebAPI.Auth;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// Logging (Serilog)
// -------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
        .WriteTo.File("Logs/Info/log-.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
        .WriteTo.File("Logs/Warning/log-.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(l => l
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
        .WriteTo.File("Logs/Error/log-.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();

builder.Host.UseSerilog();

// -------------------------------------------------
// Services
// -------------------------------------------------
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
builder.Services.AddLocalization();

var redisConn = builder.Configuration.GetSection("Cache")["RedisConfiguration"];
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

// Config binding
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CsrfOptions>(builder.Configuration.GetSection("Csrf"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));
builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<QueueOptions>(builder.Configuration.GetSection("Queue"));
builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection("RateLimiting"));

// Katman bağımlılıkları
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

// Swagger + JWT (.NET 10 Classic Style)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // JWT Tanımlaması
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token kullanın. Örn: Bearer {token}"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// Cookie ayarları
builder.Services.Configure<CookiePolicyOptions>(opt =>
{
    opt.Secure = CookieSecurePolicy.Always;
    opt.MinimumSameSitePolicy = SameSiteMode.Strict;
    opt.HttpOnly = HttpOnlyPolicy.Always;
});

// Controllers + Filters
builder.Services
    .AddControllers(o =>
    {
        o.Filters.Add<UnitOfWorkSaveChangesFilter>();
    })
    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(WebAPI.Controllers.UserController).Assembly));

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = context =>
    {
        throw new ValidationException(ResponseCodes.ValidationError);
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UnitOfWorkSaveChangesFilter>();

var app = builder.Build();

// -------------------------------------------------
// Middleware pipeline
// -------------------------------------------------
app.UseSerilogRequestLogging();
var supportedCultures = new[] { "en", "tr" };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
};

// Accept-Language header’ını dikkate al
localizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

// Database migration
using (var scope = app.Services.CreateScope())
{
    var tracker = scope.ServiceProvider.GetRequiredService<MigrationTracker>();
    await tracker.ApplyMigrationsAsync();
}

// Exception & Security Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseForwardedHeaders(); // Proxy arkasında çalışıyorsa gerçek IP'yi alabilmek için
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();
app.UseCookiePolicy();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseXssProtection(options =>
{
    options.SanitizeRequests = true;
});
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
app.MapControllers().RequireRateLimiting("RedisUserPolicy");

app.Run();
