using Congratulator.AppService.Common;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Contracts.Users;

namespace Congratulator.AppService.Users.Services;

/// <summary>
/// Сервис работы с пользователями.
/// </summary>
public interface IUserService
{
    Task<Result<ResultWithPagination<UserDTO>>> QueryAsync(UserQueryRequest request, CancellationToken ct = default);
    ValueTask<Result<UserDTO>> GetByLoginAsync(string login, CancellationToken ct = default);

    ValueTask<Result<UserDTO>> AddAsync(CreateUserRequest request, CancellationToken ct = default);
    ValueTask<Result> DeleteAsync(Guid userId, CancellationToken ct = default);
    ValueTask<Result<UserDTO>> UpdateAsync(UpdateUserRequest request, CancellationToken ct = default);
}
