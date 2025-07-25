using Congratulator.AppService.Birthdays.Services;
using Congratulator.AppService.Common;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Errors.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Congratulator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
public class BirthdaysController : ControllerBase
{
    private readonly IBirthdayService _birthdayService;
    public BirthdaysController(IBirthdayService birthdayService)
        => _birthdayService = birthdayService;

    [HttpPost("create")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(2_202_009)] // 2.1 mb limit
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IResult> CreateBirthday([FromForm] CreateBirthdayRequest model)
    {
        var result = await _birthdayService.AddAsync(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: birthdayId => Results.Ok(new { birthdayId }),
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

    [HttpGet("query")]
    [ProducesResponseType(typeof(ResultWithPagination<BirthdayDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Query([FromQuery] BirthdayQueryRequest model, CancellationToken ct)
    {
        var result = await _birthdayService.QueryAsync(model, ct);

        return result.Match(
          onSuccess: page => Results.Ok(page),
          onFailure: error => error switch
          {
              RecordNotFound nf => Results.Problem(
                                       statusCode: StatusCodes.Status404NotFound,
                                       title: nf.Code,
                                       detail: nf.Description),
              _ => Results.BadRequest(new { error.Code, error.Description })
          });
    }

    [HttpPut]
    [RequestSizeLimit(2_202_009)] // 2.1 mb limit
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(BirthdayDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IResult> UpdateBirthday([FromForm] UpdateBirthdayRequest model)
    {
        var result = await _birthdayService.UpdateAsync(model, HttpContext.RequestAborted);

        return result.Match(
            onSuccess: birthdayDto => Results.Ok(birthdayDto),
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
        var result = await _birthdayService.DeleteAsync(id, HttpContext.RequestAborted);

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
