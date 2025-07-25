using System.Linq.Expressions;

namespace Congratulator.AppService.Base;

public interface IBaseRepository<TEntity> where TEntity : class
{
    ValueTask<TEntity?> GetByIdAsync(Guid id, CancellationToken token = default);
    IQueryable<TEntity> GetFiltered(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetAll();

    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken token = default);
    Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);
    Task AddAsync(TEntity entity, CancellationToken token = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}