using Congratulator.AppService.Common;
using Congratulator.AppService.Users.Services;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Errors.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Congratulator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
[Authorize(Roles = "Admin")]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
[ProducesResponseType((int)HttpStatusCode.Forbidden)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
        => _userService = userService;

    [HttpPost("create")]
    [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IResult> CreateUser([FromForm] CreateUserRequest model)
    {
        var result = await _userService.AddAsync(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: userDto => Results.Ok(userDto),
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


    [HttpGet("query")]
    [ProducesResponseType(typeof(ResultWithPagination<UserDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IResult> GetByQuery([FromQuery] UserQueryRequest model)
    {
        var result = await _userService.QueryAsync(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: page => Results.Ok(page),
            onFailure: error =>
            {
                return Results.BadRequest(new { error.Code, error.Description });
            });
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IResult> UpdateUser([FromForm] UpdateUserRequest model)
    {
        var result = await _userService.UpdateAsync(model, HttpContext.RequestAborted);

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

    [HttpDelete("{id:Guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteAsync(id, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: () => Results.Ok(),
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
}
