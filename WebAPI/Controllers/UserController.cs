using ApplicationService.Features.Commands.CommandRequests.Users;
using ApplicationService.Features.Queries.QueryRequests.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Middleware;
using static MassTransit.ValidationResultExtensions;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());

        return CheckResponse(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromBody] GetUserByIdQuery getUserByIdQuery)
    {
        var user = await _mediator.Send(getUserByIdQuery);
        return CheckResponse(user);
    }

    [HttpPost]
    //[Idempotent(60)] // Create butonuna birkaç kez üst üste basýldýðýnda hepsi için yeni user oluþturmasýn tek bir user oluþtursun (Idempotency-Key) / 1dk cache 
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        await _mediator.Send(command);
        return Ok("User created");
    }
}



