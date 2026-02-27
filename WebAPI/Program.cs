using ApplicationService.Extensions;
using Domain.Extensions;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Persistence.Contracts;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Options;
using StackExchange.Redis;
using System.Globalization;
using System.Threading.RateLimiting;
using WebAPI.Auth;
using WebAPI.Filters;
using WebAPI.Helper;
using WebAPI.Middleware;
using RedisRateLimiting;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// Logging
// -------------------------------------------------
var serviceName = (builder.Configuration["OTEL_SERVICE_NAME"] ?? "generic-repository-pack").ToLower();
var otelEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

// Serilog Yapılandırması
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = otelEndpoint;
        options.Protocol = OtlpProtocol.Grpc;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = serviceName
        };
    })
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

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = builder.Environment.IsDevelopment() ? true : false;
            })// DB takibi
            .AddRedisInstrumentation()
            .AddOtlpExporter() // Veriyi 4317 portundaki Collector'a basar
            .SetSampler(new TraceIdRatioBasedSampler(1.0)); // Sistem prodda oturmaya başladığında zamanla düşür. 0.8 => 0.5 => 0.1
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation() // CPU/RAM gibi sistem metrikleri
            .AddOtlpExporter();
    });

// -------------------------------------------------
// Services
// -------------------------------------------------
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
builder.Services.AddLocalization();

var redisConnString = builder.Configuration.GetSection("Cache")["RedisConfiguration"];
var redisOptions = ConfigurationOptions.Parse(redisConnString);
redisOptions.AbortOnConnectFail = false;

var redisConnection = ConnectionMultiplexer.Connect(redisOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new { hata = "Too many requests. Try again later." }, cancellationToken: token);
    };

    // GLOBAL LIMIT
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if (!redisConnection.IsConnected) return RateLimitPartition.GetNoLimiter("Global");

        return RedisRateLimitPartition.GetConcurrencyRateLimiter("GlobalLimit", _ => new RedisConcurrencyRateLimiterOptions
        {
            ConnectionMultiplexerFactory = () => redisConnection,
            PermitLimit = 1000
        });
    });

    // USER POLICY
    options.AddPolicy("UserPolicy", context =>
    {
        var clientId = context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst("UserId")?.Value
            : context.Connection.RemoteIpAddress?.ToString() ?? "anon";

        if (!redisConnection.IsConnected) return RateLimitPartition.GetNoLimiter(clientId);

        return RedisRateLimitPartition.GetSlidingWindowRateLimiter(clientId, _ => new RedisSlidingWindowRateLimiterOptions
        {
            ConnectionMultiplexerFactory = () => redisConnection,
            PermitLimit = 50,
            Window = TimeSpan.FromMinutes(1)
        });
    });

    // STRICT LOGIN POLICY
    options.AddPolicy("StrictLoginPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

        if (!redisConnection.IsConnected) return RateLimitPartition.GetNoLimiter(ip);

        return RedisRateLimitPartition.GetFixedWindowRateLimiter($"login_{ip}", _ => new RedisFixedWindowRateLimiterOptions
        {
            ConnectionMultiplexerFactory = () => redisConnection,
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1)
        });
    });
});

// Config binding
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CsrfOptions>(builder.Configuration.GetSection("Csrf"));
builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));
builder.Services.Configure<ConnectionStringsOptions>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<QueueOptions>(builder.Configuration.GetSection("Queue"));

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
app.UseMiddleware<ExceptionHandlingMiddleware>();

var supportedCultures = new[] { "en", "tr" };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
};

// Accept-Language header’ını dikkate al
localizationOptions.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
app.UseForwardedHeaders(); // Proxy arkasında çalışıyorsa gerçek IP'yi alabilmek için

app.UseRouting();
app.UseRequestLocalization(localizationOptions);
app.UseCookiePolicy();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseRateLimiter();
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>();
app.UseXssProtection(options => { options.SanitizeRequests = true; });
app.UseMiddleware<CsrfMiddleware>();

// Database migration
using (var scope = app.Services.CreateScope())
{
    var tracker = scope.ServiceProvider.GetRequiredService<MigrationTracker>();
    await tracker.ApplyMigrationsAsync();
}

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
app.MapControllers().RequireRateLimiting("UserPolicy");

app.Run();
