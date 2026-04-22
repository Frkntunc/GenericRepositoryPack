using ApplicationService.Features.Commands.CommandRequests.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTOs.Auth;
using Shared.DTOs.Common;
using Shared.Options;
using WebAPI.Controllers;
using WebAPI.Model.Auth;

namespace Workify.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;
    private readonly CookieTokenOptions _cookieTokenOptions;
    private readonly CsrfOptions _csrfOptions;

    public AuthController(IMediator mediator, IOptions<CookieTokenOptions> cookieTokenOptions, IOptions<CsrfOptions> csrfOptions)
    {
        _mediator = mediator;
        _cookieTokenOptions = cookieTokenOptions.Value;
        _csrfOptions = csrfOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        );
        var result = await _mediator.Send(command, cancellationToken);

        return CheckResponse(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        string? refreshToken;

        refreshToken = Request.Cookies[_cookieTokenOptions.RefreshTokenCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return CheckResponse(ServiceResponse<RefreshTokenResponse>.Fail(ResponseCodes.InvalidRefreshToken));
        }

        var command = new RefreshTokenCommand(refreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        return CheckResponse(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] IUserContext userContext, CancellationToken cancellationToken)
    {
        Response.Cookies.Delete(_cookieTokenOptions.AccessTokenCookieName);
        Response.Cookies.Delete(_cookieTokenOptions.RefreshTokenCookieName);
        Response.Cookies.Delete(_csrfOptions.CookieName);

        var command = new LogoutCommand(userContext.UserId);
        var result = await _mediator.Send(command, cancellationToken);

        return CheckResponse(result);
    }
}


