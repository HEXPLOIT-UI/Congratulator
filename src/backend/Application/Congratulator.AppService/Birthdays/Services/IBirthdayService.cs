using Congratulator.AppService.Common;
using Congratulator.Contracts.Base;
using Congratulator.Contracts.Birthdays;

namespace Congratulator.AppService.Birthdays.Services;

/// <summary>
/// Сервис работы с днями рождений.
/// </summary>
public interface IBirthdayService
{
    Task<Result<ResultWithPagination<BirthdayDTO>>> QueryAsync(BirthdayQueryRequest request, CancellationToken ct = default);
    ValueTask<Result<Guid>> AddAsync(CreateBirthdayRequest request, CancellationToken ct = default);
    ValueTask<Result> DeleteAsync(Guid birthdayId, CancellationToken ct = default);
    ValueTask<Result<BirthdayDTO>> UpdateAsync(UpdateBirthdayRequest request, CancellationToken ct = default);
}
