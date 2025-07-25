using Congratulator.AppService.Common;
using Congratulator.Domain.Errors.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
namespace Congratulator.AppService.Files.Services;

public class PhotoStorageService : IPhotoStorageService
{
    private readonly string _imagesPath;
    private readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg", ".webp"];
    private const long MaxBytes = 2 * 1024 * 1024;
    private readonly Dictionary<string, byte[][]> FileSignatures = new()
    {
        { ".png",  new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } }, // https://copyprogramming.com/t/0x89-0x50-0x4e-0x47-0x0d-0x0a-0x1a-0x0a
        { ".jpg",  new[] { new byte[] { 0xFF, 0xD8, 0xFF } } }, // https://www.nationalarchives.gov.uk/PRONOM/Format/proFormatSearch.aspx?status=detailReport&id=670
        { ".jpeg", new[] { new byte[] { 0xFF, 0xD8, 0xFF } } }, // https://www.nationalarchives.gov.uk/PRONOM/Format/proFormatSearch.aspx?status=detailReport&id=670
        { ".webp", new[] { "RIFF"u8.ToArray() } }  // https://stackoverflow.com/questions/45321665/magic-number-for-google-image-format
    };

    public PhotoStorageService(IConfiguration configuration)
    {
        _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
    }

    public async Task<Result<string>> SaveAsync(IFormFile photo, CancellationToken ct = default)
    {
        if (photo == null || photo.Length <= 0)
            return Result<string>.Failure(new EmptyData(nameof(photo)));
        if (photo.Length > MaxBytes)
            return Result<string>.Failure(new FileTooLargeError(MaxBytes));

        if (!IsPermittedContentType(photo.ContentType))
            return Result<string>.Failure(new UnsupportedFileType($"Указан недопустимый Content Type файла - {photo.ContentType}"));

        var ext = Path.GetExtension(photo.FileName)?.ToLowerInvariant() ?? "";
        if (!(AllowedExtensions?.Contains(ext) ?? false))
            return Result<string>.Failure(new UnsupportedFileType($"Недопустимый тип файла — {ext}"));

        if (!await HasValidSignatureAsync(photo, ext, ct))
            return Result<string>.Failure(new UnsupportedFileType($"Указан файл с недопустимой сигнатурой"));

        var fileName = $"{Guid.NewGuid()}{ext}";
        Directory.CreateDirectory(_imagesPath);

        var fullPath = Path.Combine(_imagesPath, fileName);
        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await photo.CopyToAsync(stream, ct);

        return Result<string>.Success(fileName);
    }

    private bool IsPermittedContentType(string contentType)
    {
        return contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
            || contentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> HasValidSignatureAsync(IFormFile file, string ext, CancellationToken ct)
    {
        if (!FileSignatures.TryGetValue(ext, out var signatures))
            return false;

        await using var stream = file.OpenReadStream();
        var headerBytes = new byte[signatures.Max(sig => sig.Length)];
        var read = await stream.ReadAsync(headerBytes, 0, headerBytes.Length, ct);
        if (read == 0)
            return false;

        foreach (var sig in signatures)
        {
            // https://stackoverflow.com/questions/45321665/magic-number-for-google-image-format
            if (headerBytes.Take(sig.Length).SequenceEqual(sig))
            {
                if (ext == ".webp")
                {
                    var riffHeader = headerBytes.Take(4).ToArray();
                    var formatTag = headerBytes.Skip(8).Take(4).ToArray();
                    if (System.Text.Encoding.ASCII.GetString(formatTag).Equals("WEBP", StringComparison.Ordinal))
                        return true;
                    continue;
                }
                return true;
            }
        }

        return false;
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