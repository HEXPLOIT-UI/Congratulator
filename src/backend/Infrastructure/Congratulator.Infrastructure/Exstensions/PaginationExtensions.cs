using Congratulator.Contracts.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure.Exstensions;

public static class PaginationExtensions
{
    public static async Task<ResultWithPagination<TDto>> ToPaginatedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        BaseRequestWithPagination request,
        CancellationToken ct = default)
    {
        var result = new ResultWithPagination<TDto>();

        var total = await query.CountAsync(ct);
        result.AvailablePages = (int)Math.Ceiling(total / (double)request.BatchSize);

        var items = await query
            .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt"))
            .Skip(request.BatchSize * (request.Page - 1))
            .Take(request.BatchSize)
            .ProjectToType<TDto>()
            .ToListAsync(ct);

        result.Result = items;
        return result;
    }
}