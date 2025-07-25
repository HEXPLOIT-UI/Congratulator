using Congratulator.AppService.Common;
using Congratulator.Contracts.Users;

namespace Congratulator.AppService.Users.Services;

/// <summary>
/// Сервис для авторизации
/// </summary>
public interface IAuthService
{
    ValueTask<Result> RegisterUser(CreateUserRequest request, CancellationToken ct);
    ValueTask<Result> LoginUser(LoginUserRequest request, CancellationToken ct);
    ValueTask<Result<UserDTO>> GetAuthStatus(CancellationToken ct);
}
