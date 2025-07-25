using Congratulator.AppService.Base;
using Congratulator.AppService.Common;
using Congratulator.AppService.Users.Repositories;
using Congratulator.AppService.Utils;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Errors.Implementations;
using Congratulator.Domain.Users.Entity;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Congratulator.AppService.Users.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _contextAccessor;
    public AuthService(
        IUserService userService, 
        IHttpContextAccessor contextAccessor,
        IUnitOfWork unitOfWork)
    {
        _uow = unitOfWork;
        _users = (IUserRepository)_uow.Repository<UserEntity>();
        _userService = userService;
        _contextAccessor = contextAccessor;
    }

    public async ValueTask<Result<UserDTO>> GetAuthStatus(CancellationToken ct)
    {
        var userId = _contextAccessor.HttpContext!.GetUserIdFromContext();
        var userEntity = await _users.GetByIdAsync(userId);
        if (userEntity is null)
            return Result<UserDTO>.Failure(new RecordNotFound("User", userId.ToString()));
        return Result<UserDTO>.Success(userEntity.Adapt<UserDTO>());
    }

    public async ValueTask<Result> LoginUser(LoginUserRequest request, CancellationToken ct)
    {
        var userEntity = await _users.GetEntityByLoginAsync(request.Login);
        if (userEntity == null)
            return Result.Failure(new RecordNotFound("User", request.Login));
        if (!HashUtils.VerifyPassword(request.Password, userEntity.PasswordHash))
            return Result.Failure(new AuthorizationFailed("Неверный пароль"));
        await CreateSession(userEntity.Adapt<UserDTO>());
        return Result.Success();
    }

    public async ValueTask<Result> RegisterUser(CreateUserRequest request, CancellationToken ct)
    {
        var createResult = await _userService.AddAsync(request, ct);
        if (createResult.IsFailure)
            return createResult;
        await CreateSession(createResult.Value);
        return Result.Success();
    }

    private async Task CreateSession(UserDTO userDTO)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userDTO.EntityId.ToString()),
                new(ClaimTypes.Role, userDTO.Role)
            };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddHours(24)
        };
        await _contextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }
}
