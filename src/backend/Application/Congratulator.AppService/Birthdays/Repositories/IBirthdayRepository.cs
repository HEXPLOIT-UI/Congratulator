using Congratulator.AppService.Base;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;
using Congratulator.Domain.Birthdays.Entity;

namespace Congratulator.AppService.Birthdays.Repositories;


/// <summary>
/// Репозиторий для работы с <see cref="BirthdayEntity"/>.
/// </summary>
public interface IBirthdayRepository : IBaseRepository<BirthdayEntity>
{
    Task<ResultWithPagination<BirthdayDTO>> QueryAsync(BirthdayQueryRequest req, CancellationToken ct = default);
    Task<ResultWithPagination<BirthdayDTO>> QueryAsync(Guid userId, BirthdayQueryRequest req, CancellationToken ct = default);
    ValueTask<Guid> AddAsync(Guid userId, CreateBirthdayRequest request, string? imagePath = null, CancellationToken ct = default);
    ValueTask<BirthdayDTO> UpdateAsync(UpdateBirthdayRequest request, string? imagePath = null, CancellationToken ct = default);
    Task DeleteAsync(Guid entityId, CancellationToken ct = default);
}
