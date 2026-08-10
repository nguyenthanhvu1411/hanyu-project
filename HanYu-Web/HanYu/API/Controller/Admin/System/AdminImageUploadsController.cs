using HanYu.Application.Interfaces.Storage;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.System;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/uploads")]
public sealed class AdminMediaUploadsController : ControllerBase
{
    private const long MaxImageSize = 10L * 1024 * 1024;
    private const long MaxAudioSize = 50L * 1024 * 1024;
    private const long MaxVideoSize = 200L * 1024 * 1024;
    private const long MaxDocumentSize = 50L * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> ImageTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif",
            ["image/avif"] = ".avif"
        };

    private static readonly IReadOnlyDictionary<string, string> AudioTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["audio/mpeg"] = ".mp3",
            ["audio/mp4"] = ".m4a",
            ["audio/wav"] = ".wav",
            ["audio/x-wav"] = ".wav",
            ["audio/ogg"] = ".ogg",
            ["audio/webm"] = ".webm",
            ["audio/aac"] = ".aac",
            ["audio/flac"] = ".flac"
        };

    private static readonly IReadOnlyDictionary<string, string> VideoTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["video/mp4"] = ".mp4",
            ["video/webm"] = ".webm",
            ["video/ogg"] = ".ogv",
            ["video/quicktime"] = ".mov",
            ["video/x-matroska"] = ".mkv"
        };

    private static readonly IReadOnlyDictionary<string, string> DocumentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = ".pdf",
            ["text/plain"] = ".txt",
            ["text/csv"] = ".csv",
            ["text/vtt"] = ".vtt",
            ["application/msword"] = ".doc",
            ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = ".docx",
            ["application/vnd.ms-powerpoint"] = ".ppt",
            ["application/vnd.openxmlformats-officedocument.presentationml.presentation"] = ".pptx",
            ["application/vnd.ms-excel"] = ".xls",
            ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = ".xlsx"
        };

    private readonly IPublicFileStorage _storage;

    public AdminMediaUploadsController(IPublicFileStorage storage)
    {
        _storage = storage;
    }

    [HttpPost("images")]
    [RequestSizeLimit(MaxImageSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSize)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<AdminMediaUploadResponse>> UploadImage(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
        => UploadAsync(
            file,
            MediaKind.Image,
            ImageTypes,
            MaxImageSize,
            "images",
            cancellationToken);

    [HttpPost("audio")]
    [RequestSizeLimit(MaxAudioSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxAudioSize)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<AdminMediaUploadResponse>> UploadAudio(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
        => UploadAsync(
            file,
            MediaKind.Audio,
            AudioTypes,
            MaxAudioSize,
            "audio",
            cancellationToken);

    [HttpPost("videos")]
    [RequestSizeLimit(MaxVideoSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxVideoSize)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<AdminMediaUploadResponse>> UploadVideo(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
        => UploadAsync(
            file,
            MediaKind.Video,
            VideoTypes,
            MaxVideoSize,
            "videos",
            cancellationToken);

    [HttpPost("documents")]
    [RequestSizeLimit(MaxDocumentSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxDocumentSize)]
    [Consumes("multipart/form-data")]
    public Task<ActionResult<AdminMediaUploadResponse>> UploadDocument(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
        => UploadAsync(
            file,
            MediaKind.Document,
            DocumentTypes,
            MaxDocumentSize,
            "documents",
            cancellationToken);

    private async Task<ActionResult<AdminMediaUploadResponse>> UploadAsync(
        IFormFile file,
        MediaKind kind,
        IReadOnlyDictionary<string, string> allowedContentTypes,
        long maxFileSize,
        string folder,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tệp không hợp lệ",
                Detail = "Vui lòng chọn một tệp để tải lên.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (file.Length > maxFileSize)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tệp quá lớn",
                Detail = $"Dung lượng tối đa cho {GetKindLabel(kind)} là {FormatMegabytes(maxFileSize)} MB.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!allowedContentTypes.TryGetValue(file.ContentType, out var extension))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Định dạng tệp không được hỗ trợ",
                Detail = $"MIME type '{file.ContentType}' không được phép cho {GetKindLabel(kind)}.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var objectKey = $"{folder}/{DateTime.UtcNow:yyyy/MM}/{fileName}";

        await using var stream = file.OpenReadStream();
        var uploaded = await _storage.UploadAsync(
            objectKey,
            stream,
            file.ContentType,
            cancellationToken);

        return Ok(new AdminMediaUploadResponse(
            uploaded.PublicUrl,
            uploaded.ObjectKey,
            fileName,
            file.ContentType,
            file.Length,
            kind.ToString().ToLowerInvariant()));
    }

    private static long FormatMegabytes(long bytes)
        => bytes / (1024 * 1024);

    private static string GetKindLabel(MediaKind kind)
        => kind switch
        {
            MediaKind.Image => "ảnh",
            MediaKind.Audio => "audio",
            MediaKind.Video => "video",
            MediaKind.Document => "tài liệu",
            _ => "tệp"
        };

    private enum MediaKind
    {
        Image,
        Audio,
        Video,
        Document
    }
}

public sealed record AdminMediaUploadResponse(
    string Url,
    string ObjectKey,
    string FileName,
    string ContentType,
    long Size,
    string Kind);
