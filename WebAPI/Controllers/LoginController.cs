using Shared.Enums;
using ApplicationService.Services;
using ApplicationService.SharedKernel;
using ApplicationService.SharedKernel.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Model.Auth;
using WebAPI.Model.Login;
using WebAPI.Helper;
using ApplicationService.Repositories;
using Shared.Exceptions;
using Shared.Constants;

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

}


