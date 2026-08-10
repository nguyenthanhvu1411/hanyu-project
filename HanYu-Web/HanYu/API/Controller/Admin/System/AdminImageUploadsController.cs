using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.System;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/uploads/images")]
public sealed class AdminImageUploadsController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> AllowedContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif"
        };

    private readonly IWebHostEnvironment _environment;

    public AdminImageUploadsController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost]
    [RequestSizeLimit(MaxFileSize)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AdminImageUploadResponse>> Upload(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tệp ảnh không hợp lệ",
                Detail = "Vui lòng chọn một tệp ảnh để tải lên.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Tệp ảnh quá lớn",
                Detail = "Dung lượng ảnh tối đa là 5 MB.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!AllowedContentTypes.TryGetValue(file.ContentType, out var extension))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Định dạng ảnh không được hỗ trợ",
                Detail = "Chỉ hỗ trợ JPG, PNG, WEBP hoặc GIF.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var uploadDirectory = Path.Combine(webRoot, "uploads", "images");
        Directory.CreateDirectory(uploadDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var destinationPath = Path.Combine(uploadDirectory, fileName);

        await using (var stream = new FileStream(
                         destinationPath,
                         FileMode.CreateNew,
                         FileAccess.Write,
                         FileShare.None,
                         81920,
                         useAsync: true))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var relativeUrl = $"/uploads/images/{fileName}";
        var absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";

        return Ok(new AdminImageUploadResponse(
            absoluteUrl,
            relativeUrl,
            fileName,
            file.ContentType,
            file.Length));
    }
}

public sealed record AdminImageUploadResponse(
    string Url,
    string RelativeUrl,
    string FileName,
    string ContentType,
    long Size);
