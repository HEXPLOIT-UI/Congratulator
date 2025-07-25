using Congratulator.AppService.Base;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Users.Entity;

namespace Congratulator.AppService.Users.Repositories;

/// <summary>
/// Репозиторий для работы с <see cref="UserEntity"/>.
/// </summary>
public interface IUserRepository : IBaseRepository<UserEntity>
{
    Task<ResultWithPagination<UserDTO>> QueryAsync(UserQueryRequest request, CancellationToken ct = default);
    ValueTask<Guid> AddAsync(CreateUserRequest request, CancellationToken ct = default);
    ValueTask<UserDTO?> GetByLoginAsync(string login, CancellationToken ct = default);
    ValueTask<UserEntity?> GetEntityByLoginAsync(string login, CancellationToken ct = default);
    ValueTask<UserDTO> UpdateAsync(UpdateUserRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid entityId, CancellationToken ct = default);

}
