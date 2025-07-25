using Congratulator.AppService.Birthdays.Repositories;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Domain.Birthdays.Entity;
using Congratulator.Infrastructure.Base;
using Congratulator.Infrastructure.Exstensions;
using LinqKit;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Congratulator.Infrastructure.Birthdays.Repositories;

public class BirthdayRepository : BaseRepository<BirthdayEntity>, IBirthdayRepository
{
    private readonly ApplicationDbContext _ctx;

    public BirthdayRepository(ApplicationDbContext context) : base(context)
    {
        _ctx = context;
    }

    public async ValueTask<Guid> AddAsync(Guid userId, CreateBirthdayRequest request, string? image, CancellationToken ct = default)
    {
        var entity = request.Adapt<BirthdayEntity>();
        entity.EntityId = Guid.NewGuid();
        entity.UserId = userId;
        entity.PhotoPath = image;
        await _ctx.Birthdays.AddAsync(entity, ct);
        return entity.EntityId;
    }

    public async Task DeleteAsync(Guid entityId, CancellationToken ct = default)
    {
        var entity = await GetFiltered(x => x.EntityId.Equals(entityId)).FirstOrDefaultAsync(ct)
            ?? throw new ArgumentNullException($"Birthday entity with id '{entityId}' not found");
        Delete(entity);
    }

    public Task<ResultWithPagination<BirthdayDTO>> QueryAsync(BirthdayQueryRequest req, CancellationToken ct = default)
    {
        var predicate = PredicateBuilder.New<BirthdayEntity>(true);
        ApplyCommonFilters(predicate, req, ct);
        var query = GetAll()
                        .AsExpandable()
                        .Where(predicate);
        return query.ToPaginatedResultAsync<BirthdayEntity, BirthdayDTO>(req, ct);
    }

    public Task<ResultWithPagination<BirthdayDTO>> QueryAsync(Guid userId, BirthdayQueryRequest req, CancellationToken ct = default)
    {
        var predicate = PredicateBuilder.New<BirthdayEntity>(x => x.UserId == userId);
        ApplyCommonFilters(predicate, req, ct);
        var query = GetAll()
                        .AsExpandable()
                        .Where(predicate);
        return query.ToPaginatedResultAsync<BirthdayEntity, BirthdayDTO>(req, ct);
    }

    public async ValueTask<BirthdayDTO> UpdateAsync(UpdateBirthdayRequest request, string? imagePath, CancellationToken ct = default)
    {
        var entity = await GetFiltered(x => x.EntityId.Equals(request.EntityId)).FirstOrDefaultAsync(ct)
           ?? throw new ArgumentNullException($"Birthday entity with id '{request.EntityId}' not found");
        request.Adapt(entity);
        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            entity.PhotoPath = imagePath;
        }
        Update(entity);
        return entity.Adapt<BirthdayDTO>();
    }

    private ExpressionStarter<BirthdayEntity> ApplyCommonFilters(ExpressionStarter<BirthdayEntity> predicate, BirthdayQueryRequest req, CancellationToken ct)
    {
        if (req.ActiveOnly == true)
            predicate = predicate.And(x => x.IsActive);

        if (req.IncomingOnly == true)
        {
            DateTime today = DateTime.Today.ToUniversalTime();
            DateTime nextWeek = today.AddDays(7);

            predicate = predicate.And(x =>
                (x.DateOfBirth.Month == today.Month && x.DateOfBirth.Day >= today.Day) || 
                (x.DateOfBirth.Month == nextWeek.Month && x.DateOfBirth.Day <= nextWeek.Day && nextWeek.Month != today.Month));
        }
            
        if (!string.IsNullOrWhiteSpace(req.FirstName))
            predicate = predicate.And(x => x.FirstName
                                             .Contains(req.FirstName));
        if (!string.IsNullOrWhiteSpace(req.LastName))
            predicate = predicate.And(x => x.LastName
                                             .Contains(req.LastName));

        if (!string.IsNullOrWhiteSpace(req.Description))
            predicate = predicate.And(x => x.Description != null && x.Description
                                             .Contains(req.Description));

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            predicate = predicate.And(x =>
                x.FirstName.Contains(req.Search) ||
                x.LastName.Contains(req.Search) ||
                (x.Description != null && x.Description.Contains(req.Search)));
        }

        return predicate;
    }
}
