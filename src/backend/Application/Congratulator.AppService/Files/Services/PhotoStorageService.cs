using Congratulator.AppService.Common;
using Congratulator.Domain.Errors.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
namespace Congratulator.AppService.Files.Services;

public class PhotoStorageService : IPhotoStorageService
{
    private readonly string _imagesPath;
    private const long MaxBytes = 2 * 1024 * 1024; 

    public PhotoStorageService(IConfiguration configuration)
    {
        _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
    }

    public async Task<Result<string>> SaveAsync(IFormFile photo, CancellationToken ct = default)
    {
        if (photo.Length > MaxBytes)
            return Result<string>.Failure(new FileTooLargeError(MaxBytes));

        var ext = Path.GetExtension(photo.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        Directory.CreateDirectory(_imagesPath);

        var fullPath = Path.Combine(_imagesPath, fileName);
        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await photo.CopyToAsync(stream, ct);

        return Result<string>.Success(fileName);
    }

    public Result Delete(string? fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            var fullPath = Path.Combine(_imagesPath, fileName);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        return Result.Success();
    }
}