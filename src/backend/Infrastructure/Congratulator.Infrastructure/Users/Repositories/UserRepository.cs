using Congratulator.AppService.Users.Repositories;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Contracts.Users;
using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Domain.Users.Entity;
using Congratulator.Infrastructure.Base;
using Congratulator.Infrastructure.Exstensions;
using LinqKit;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure.Users.Repositories;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    private readonly ApplicationDbContext _ctx;
    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _ctx = context;
    }

    public async ValueTask<Guid> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userEntity = request.Adapt<UserEntity>();
        await AddAsync(userEntity, cancellationToken);
        return userEntity.EntityId;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userEntity = await GetFiltered(x => x.EntityId.Equals(userId)).FirstOrDefaultAsync(cancellationToken) 
            ?? throw new ArgumentNullException($"User entity with id '{userId}' not found");
        Delete(userEntity);
    }

    public async ValueTask<UserDTO> UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userEntity = await GetFiltered(x => x.EntityId.Equals(request.UserId)).FirstOrDefaultAsync(cancellationToken)
           ?? throw new ArgumentNullException($"User entity with id '{request.UserId}' not found");
        request.Adapt(userEntity);
        Update(userEntity);
        return userEntity.Adapt<UserDTO>();
    }

    public async ValueTask<UserDTO?> GetByLoginAsync(string login, CancellationToken ct = default)
    {
        var user = await GetFiltered(u => EF.Functions.ILike(u.Login, login))
             .FirstOrDefaultAsync(ct);

        return user?.Adapt<UserDTO>();
    }

    public async ValueTask<UserEntity?> GetEntityByLoginAsync(string login, CancellationToken ct = default)
    {
        return await _ctx.Users
                         .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Login, login), ct);
    }

    public Task<ResultWithPagination<UserDTO>> QueryAsync(UserQueryRequest request, CancellationToken ct = default)
    {
        var predicate = PredicateBuilder.New<UserEntity>(true);

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            predicate = predicate.And(x => x.FirstName
                                             .Contains(request.FirstName));
        if (!string.IsNullOrWhiteSpace(request.LastName))
            predicate = predicate.And(x => x.LastName
                                             .Contains(request.LastName));

        if (!string.IsNullOrWhiteSpace(request.Login))
            predicate = predicate.And(x => x.Login
                                             .Contains(request.Login));

        if (!string.IsNullOrWhiteSpace(request.TelegramId))
            predicate = predicate.And(x => x.TelegramId != null && x.TelegramId
                                             .Contains(request.TelegramId));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            predicate = predicate.And(x =>
                x.FirstName.Contains(request.Search) ||
                x.LastName.Contains(request.Search) ||
                x.Login.Contains(request.Search));
        }
        var query = GetAll()
                        .AsExpandable()  
                        .Where(predicate);

        return query.ToPaginatedResultAsync<UserEntity, UserDTO>(request, ct);
    }
}
