using Congratulator.AppService.Users.Services;
using Congratulator.Contracts.Users;
using Congratulator.AppService.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Congratulator.Domain.Errors.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace Congratulator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("login")]
    public async Task<IResult> SignIn([FromForm] LoginUserRequest model)
    {
        var result = await _authService.LoginUser(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: error =>
            {
                return error switch
                {
                    AuthorizationFailed authFailed =>
                        Results.Problem(
                            statusCode: StatusCodes.Status403Forbidden,
                            title: authFailed.Code,
                            detail: authFailed.Description),

                    _ => Results.BadRequest(new { error.Code, error.Description })
                };
            });
    }

    [HttpPost("register")]
    public async Task<IResult> SignUp([FromForm] CreateUserRequest model)
    {
        var result = await _authService.RegisterUser(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: () => Results.Ok(),
            onFailure: error =>
            {
                return error switch
                {
                    RecordAlreadyExisted alreadyExisted =>
                        Results.Problem(
                            statusCode: StatusCodes.Status409Conflict,
                            title: alreadyExisted.Code,
                            detail: alreadyExisted.Description),

                    _ => Results.BadRequest(new { error.Code, error.Description })
                };
            });
    }

    [HttpGet("check")]
    [Authorize]
    public async Task<IResult> CheckAuthStatus()
    {
        var result = await _authService.GetAuthStatus(HttpContext.RequestAborted);

        return result.Match(
            onSuccess: userDto => Results.Ok(userDto),
            onFailure: error =>
            {
                return error switch
                {
                    RecordNotFound notFound =>
                        Results.Problem(
                            statusCode: StatusCodes.Status404NotFound,
                            title: notFound.Code,
                            detail: notFound.Description),

                    _ => Results.BadRequest(new { error.Code, error.Description })
                };
            });
    }

    [HttpPost("logout")]
    public async Task<IResult> Logout()
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        return Results.Ok();
    }
}
