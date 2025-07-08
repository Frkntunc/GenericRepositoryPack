using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Serilog;
using FluentValidation;
using WebAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using ApplicationService.SharedKernel.Exceptions;
using ApplicationService.SharedKernel.Auth;
using WebApiGateway.Middleware;
using ApplicationService.SharedKernel.Auth.Common;
using ApplicationService.SharedKernel;
using Infrastructure.Extensions;
using ApplicationService.Services;
using ApplicationService.Repositories;
using Users.Infrastructure.Repositories;
using Infrastructure.Filters;
using ApplicationService.Repositories.Common;
using Infrastructure.Repositories.Common;
using Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using ApplicationService.Features.Queries.QueryHandlers.User;
using ApplicationService.Features.Queries.QueryRequests.User;
using Domain.Models;
using MediatR;
using ApplicationService.Extensions;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServiceForWrite(builder.Configuration);
builder.Services.AddInfrastructureServiceForRead(builder.Configuration);
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
//builder.Services.AddMediatR(typeof(Program));  // veya assembly typeof(CreateUserCommand).Assembly
//builder.Services.AddScoped<IRequestHandler<CreateUserCommand, Unit>, CreateUserCommandHandler>();
//builder.Services.AddScoped<IRequestHandler<GetAllUsersQuery, List<UserReadModel>>, GetAllUsersQueryHandler>();

builder.Services
    .AddControllers()
    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(WebAPI.Controllers.UserController).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<JwtCookieMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();
app.UseMiddleware<CsrfMiddleware>();

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
