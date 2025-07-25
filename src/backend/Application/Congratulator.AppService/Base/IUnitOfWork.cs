namespace Congratulator.AppService.Base;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<TEntity> Repository<TEntity>() where TEntity : class;
    ValueTask<int> SaveChangesAsync(CancellationToken token = default);
    Task BeginTransactionAsync(CancellationToken token = default);
    Task CommitAsync(CancellationToken token = default);
    Task RollbackAsync(CancellationToken token = default);
}