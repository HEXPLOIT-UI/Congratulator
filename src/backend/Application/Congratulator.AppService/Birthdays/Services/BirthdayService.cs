using Congratulator.AppService.Base;
using Congratulator.AppService.Birthdays.Repositories;
using Congratulator.AppService.Common;
using Congratulator.AppService.Files.Services;
using Congratulator.AppService.Users.Repositories;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Domain.Errors.Implementations;
using Congratulator.Domain.Users.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Congratulator.AppService.Birthdays.Services;

public class BirthdayService : IBirthdayService
{
    private readonly IUnitOfWork _uow;
    private readonly IBirthdayRepository _birthdays;
    private readonly IUserRepository _users;
    private readonly IPhotoStorageService _photoService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<BirthdayService> _logger;

    public BirthdayService(
        IUnitOfWork uow,
        IHttpContextAccessor contextAccessor,
        ILogger<BirthdayService> logger,
        IPhotoStorageService photoStorageService)
    {
        _uow = uow;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _photoService = photoStorageService;
        _birthdays = (IBirthdayRepository)_uow.Repository<BirthdayEntity>();
        _users = (IUserRepository)_uow.Repository<UserEntity>();
    }

    public async ValueTask<Result<Guid>> AddAsync(CreateBirthdayRequest request, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var userId = _contextAccessor.HttpContext!.GetUserIdFromContext();
            if (await _users.GetByIdAsync(userId) is null)
                return Result<Guid>.Failure(new RecordNotFound("User", userId.ToString()));

            string? fileName = null;
            if (request.Photo is not null)
            {
                var result = await _photoService.SaveAsync(request.Photo, ct);
                if (result.IsFailure)
                    return Result<Guid>.Failure(result.Error!);
                fileName = result.Value;
            }

            var newId = await _birthdays.AddAsync(userId, request, fileName, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            return Result<Guid>.Success(newId);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask<Result<BirthdayDTO>> UpdateAsync(UpdateBirthdayRequest request, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var userId = _contextAccessor.HttpContext!.GetUserIdFromContext();
            var userEntity = await _users.GetByIdAsync(userId);
            if (userEntity is null)
                return Result<BirthdayDTO>.Failure(new RecordNotFound("User", userId.ToString()));
            var birthdayEntity = await _birthdays.GetByIdAsync(request.EntityId);
            if (birthdayEntity is null || !birthdayEntity.UserId.Equals(userId))
                return Result<BirthdayDTO>.Failure(new RecordNotFound("Birthday", request.EntityId.ToString()));

            string? newFileName = null;
            if (request.Photo is not null)
            {
                var result = await _photoService.SaveAsync(request.Photo, ct);
                if (result.IsFailure)
                    return Result<BirthdayDTO>.Failure(result.Error!);
                newFileName = result.Value;
                _photoService.Delete(birthdayEntity.PhotoPath);
            }

            var updatedBirthday = await _birthdays.UpdateAsync(request, newFileName, ct: ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
            return Result<BirthdayDTO>.Success(updatedBirthday);
        }
        catch (ArgumentNullException)
        {
            return Result<BirthdayDTO>.Failure(new RecordNotFound("Birthday", request.EntityId.ToString()));
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async ValueTask<Result> DeleteAsync(Guid birthdayId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var userId = _contextAccessor.HttpContext!.GetUserIdFromContext();
            var userEntity = await _users.GetByIdAsync(userId);
            if (userEntity is null)
                return Result<Guid>.Failure(new RecordNotFound("User", userId.ToString()));
            var birthdayEntity = await _birthdays.GetByIdAsync(birthdayId);
            if (birthdayEntity is null || !birthdayEntity.UserId.Equals(userId))
                return Result<BirthdayDTO>.Failure(new RecordNotFound("Birthday", birthdayId.ToString()));
            if (!string.IsNullOrWhiteSpace(birthdayEntity.PhotoPath))
            {
                _photoService.Delete(birthdayEntity.PhotoPath);
            }
            await _birthdays.DeleteAsync(birthdayId, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
            return Result.Success();
        }
        catch (ArgumentNullException)
        {
            return Result<UserDTO>.Failure(new RecordNotFound("Birthday", birthdayId.ToString()));
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<ResultWithPagination<BirthdayDTO>>> QueryAsync(
         BirthdayQueryRequest request, CancellationToken ct)
    {
        var userId = _contextAccessor.HttpContext!.GetUserIdFromContext();

        var user = await _uow.Repository<UserEntity>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<ResultWithPagination<BirthdayDTO>>
                      .Failure(new RecordNotFound("User", userId.ToString()));

        var page = await _birthdays.QueryAsync(userId, request, ct);
        return Result<ResultWithPagination<BirthdayDTO>>.Success(page);
    }
}
