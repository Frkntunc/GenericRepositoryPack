using Domain.Enums;
using ApplicationService.Services;
using ApplicationService.SharedKernel;
using ApplicationService.SharedKernel.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using ApplicationService.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebAPI.Model.Auth;
using WebAPI.Model.Login;
using WebAPI.Helper;
using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : BaseController
{
    private readonly JwtTokenService tokenService;
    private readonly RefreshTokenService refreshTokenService;
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasherService passwordHasherService;

    public LoginController(JwtTokenService tokenService, RefreshTokenService refreshTokenService, IUserRepository userRepository, IPasswordHasherService passwordHasherService)
    {
        this.tokenService = tokenService;
        this.refreshTokenService = refreshTokenService;
        this.userRepository = userRepository;
        this.passwordHasherService = passwordHasherService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("Mail veya þifre hatalý", ErrorCodes.UserNotFound);

        var passwordValid = passwordHasherService.VerifyHashedPassword(user.PasswordHash, request.Password);

        if (passwordValid)
        {
            var userId = "1";
            var email = "furkan@example.com";
            var role = "Admin";

            var accessToken = tokenService.GenerateToken(userId, email, role);
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(userId);

            CookieHelper.SetAuthCookies(Response, accessToken, refreshToken.Token);

            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token
            });
        }
        else
        {
            throw new NotFoundException("Mail veya þifre hatalý", ErrorCodes.UserNotFound);
        }
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model, [FromServices] IUserContext userContext)
    {
        var newRefreshToken = await refreshTokenService.RotateRefreshTokenAsync(model.RefreshToken, userContext.UserId);

        if (newRefreshToken == null)
            throw new UnauthorizedException("Refresh token geçersiz veya süresi dolmuþ.", ErrorCodes.InvalidRefreshToken);

        var newAccessToken = tokenService.GenerateToken(userContext.UserId, userContext.Email, userContext.Role);

        CookieHelper.SetAuthCookies(Response, newAccessToken, newRefreshToken.Token);
        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromServices] IUserContext userContext)
    {
        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        await refreshTokenService.RevokeAllTokensByUserId(userContext.UserId);

        return Ok();
    }

}


