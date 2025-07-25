using Congratulator.AppService.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Congratulator.Infrastructure.Base;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    public BaseRepository(ApplicationDbContext context) => _context = context;

    public async ValueTask<T?> GetByIdAsync(Guid id, CancellationToken token = default)
        => await _context.Set<T>().FindAsync([id], token);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken token = default)
        => await _context.Set<T>().ToListAsync(token);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
        => await _context.Set<T>().Where(predicate).ToListAsync(token);

    public async Task AddAsync(T entity, CancellationToken token = default)
        => await _context.Set<T>().AddAsync(entity, token);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);

    public IQueryable<T> GetFiltered(Expression<Func<T, bool>> predicate)
        => _context.Set<T>().Where(predicate).AsNoTracking();

    public IQueryable<T> GetAll()
        => _context.Set<T>().AsNoTracking();
}