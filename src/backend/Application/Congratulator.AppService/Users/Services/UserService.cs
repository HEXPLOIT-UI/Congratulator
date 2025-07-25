using Congratulator.AppService.Base;
using Congratulator.AppService.Common;
using Congratulator.AppService.Users.Repositories;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Errors.Implementations;
using Congratulator.Domain.Users.Entity;
using Mapster;
using System.Threading;

namespace Congratulator.AppService.Users.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IUserRepository _users;

    public UserService(IUnitOfWork uow)
    {
        _uow = uow;
        _users = (IUserRepository)_uow.Repository<UserEntity>();
    }

    public async ValueTask<Result<UserDTO>> AddAsync(CreateUserRequest req, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            if (await _users.GetByLoginAsync(req.Login, ct) is not null)
                return Result<UserDTO>.Failure(new RecordAlreadyExisted("User", req.Login));

            var newId = await _users.AddAsync(req, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
            var entity = await _users.GetByIdAsync(newId);
            return Result<UserDTO>.Success(entity.Adapt<UserDTO>());
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask<Result> DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            await _users.DeleteAsync(userId, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result<UserDTO>.Failure(new RecordNotFound("User", userId.ToString()));
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask<Result<UserDTO>> GetByLoginAsync(string login, CancellationToken ct = default)
    {
        var userEntity = await _users.GetByLoginAsync(login, ct);
        if (userEntity is null)
            return Result<UserDTO>.Failure(new RecordNotFound("User", login));

        return Result<UserDTO>.Success(userEntity);
    }

    public async Task<Result<ResultWithPagination<UserDTO>>> QueryAsync(UserQueryRequest request, CancellationToken ct = default)
    {
        var page = await _users.QueryAsync(request, ct);
        return Result<ResultWithPagination<UserDTO>>.Success(page);
    }

    public async ValueTask<Result<UserDTO>> UpdateAsync(UpdateUserRequest request, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var updateUser = await _users.UpdateAsync(request, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
            return Result<UserDTO>.Success(updateUser);
        }
        catch (ArgumentNullException)
        {
            return Result<UserDTO>.Failure(new RecordNotFound("User", request.UserId.ToString()));
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }
}
