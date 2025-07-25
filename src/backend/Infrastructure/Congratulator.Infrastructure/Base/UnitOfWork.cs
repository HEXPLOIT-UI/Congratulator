using Congratulator.AppService.Base;
using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Domain.Users.Entity;
using Congratulator.Infrastructure.Birthdays.Repositories;
using Congratulator.Infrastructure.Exstensions;
using Congratulator.Infrastructure.Users.Repositories;

namespace Congratulator.Infrastructure.Base;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = [];

    private static readonly Dictionary<Type, Type> _map = new()
    {
        [typeof(UserEntity)] = typeof(UserRepository),
        [typeof(BirthdayEntity)] = typeof(BirthdayRepository)
    };

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IBaseRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var entityType = typeof(TEntity);
        if (!_repositories.TryGetValue(entityType, out var repo))
        {
            var repoType = _map.TryGetValue(entityType, out var impl)
                ? impl.MakeGenericIfNeeded<TEntity>()
                : typeof(BaseRepository<TEntity>);

            repo = Activator.CreateInstance(repoType, _context)
                   ?? throw new InvalidOperationException($"Cannot create {repoType.Name}");

            _repositories[entityType] = repo;
        }
        return (IBaseRepository<TEntity>)repo;
    }

    public async ValueTask<int> SaveChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);

    public async Task BeginTransactionAsync(CancellationToken token = default)
        => await _context.Database.BeginTransactionAsync(token);

    public async Task CommitAsync(CancellationToken token = default)
    {
        await _context.Database.CommitTransactionAsync(token);
    }

    public async Task RollbackAsync(CancellationToken token = default)
    {
        await _context.Database.RollbackTransactionAsync(token);
    }

    public void Dispose() => _context.Dispose();
}