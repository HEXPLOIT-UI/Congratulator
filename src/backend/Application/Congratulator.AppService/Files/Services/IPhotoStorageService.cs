using Congratulator.AppService.Common;
using Microsoft.AspNetCore.Http;

namespace Congratulator.AppService.Files.Services;

public interface IPhotoStorageService
{
    Task<Result<string>> SaveAsync(IFormFile photo, CancellationToken ct = default);
    Result Delete(string? fileName);
}