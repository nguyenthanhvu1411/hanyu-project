using HanYu.Domain.Constants;
using HanYu.Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HanYu.API.Controller.Admin;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/uploads")]
public sealed class UploadsController : ControllerBase
{
    private const long MaxAudioBytes = 25L * 1024 * 1024;

    private static readonly HashSet<string> AllowedAudioExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".m4a", ".wav", ".ogg", ".webm", ".aac"
        };

    private readonly IOptions<StorageOptions> _storageOptions;

    public UploadsController(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions;
    }

    [HttpPost("audio")]
    [RequestSizeLimit(MaxAudioBytes + 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxAudioBytes + 1024 * 1024)]
    public async Task<IActionResult> UploadAudio(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length <= 0)
        {
            return BadRequest(new
            {
                code = "Upload.AudioRequired",
                message = "Hãy chọn một file audio để tải lên."
            });
        }

        if (file.Length > MaxAudioBytes)
        {
            return BadRequest(new
            {
                code = "Upload.AudioTooLarge",
                message = "File audio không được vượt quá 25 MB."
            });
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) ||
            !AllowedAudioExtensions.Contains(extension))
        {
            return BadRequest(new
            {
                code = "Upload.AudioExtensionNotAllowed",
                message = "Chỉ hỗ trợ MP3, M4A, WAV, OGG, WEBM hoặc AAC."
            });
        }

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? GetFallbackContentType(extension)
            : file.ContentType.Trim().ToLowerInvariant();

        if (!contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(contentType, "video/webm", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                code = "Upload.InvalidAudioMimeType",
                message = "MIME type của file không phải audio hợp lệ."
            });
        }

        var objectKey = string.Join('/',
            "audio",
            "vocabulary",
            DateTime.UtcNow.ToString("yyyy"),
            DateTime.UtcNow.ToString("MM"),
            $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}");

        await using var stream = file.OpenReadStream();
        using var storage = new S3PublicFileStorage(_storageOptions);
        var uploaded = await storage.UploadAsync(
            objectKey,
            stream,
            contentType,
            cancellationToken);

        return Ok(new
        {
            objectKey = uploaded.ObjectKey,
            publicUrl = uploaded.PublicUrl,
            originalFileName = Path.GetFileName(file.FileName),
            contentType,
            fileSizeBytes = file.Length
        });
    }

    private static string GetFallbackContentType(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".mp3" => "audio/mpeg",
            ".m4a" => "audio/mp4",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".webm" => "audio/webm",
            ".aac" => "audio/aac",
            _ => "application/octet-stream"
        };
}
