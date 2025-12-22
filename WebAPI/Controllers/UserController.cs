using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Features.Queries.QueryRequests.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebAPI.CustomAttribute;
using WebAPI.Middleware;
using static MassTransit.ValidationResultExtensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Information log");
        _logger.LogError("Error log");
        Log.Information("Informationasdsad log");
        Log.Error("Errorasdasdas log");
        var users = await _mediator.Send(new GetAllUsersQuery());

        return CheckResponse(users);
    }

    [HasPermission("getuser")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromBody] GetUserByIdQuery getUserByIdQuery)
    {
        var user = await _mediator.Send(getUserByIdQuery);
        return CheckResponse(user);
    }

    [HasPermission("createuser")]
    [HttpPost]
    //[Idempotent(60)] // Create butonuna birkaç kez üst üste basýldýðýnda hepsi için yeni user oluþturmasýn tek bir user oluþtursun (Idempotency-Key) / 1dk cache 
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        await _mediator.Send(command);
        return Ok("User created");
    }
}



