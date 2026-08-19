using System.Text;
using HanYu.Application.Interfaces.Storage;
using HanYu.Domain.Constants;
using HanYu.Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HanYu.API.Controller.Admin.System;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/storage")]
public sealed class AdminStorageController : ControllerBase
{
    private readonly IPublicFileStorage _storage;
    private readonly StorageOptions _options;

    public AdminStorageController(
        IPublicFileStorage storage,
        IOptions<StorageOptions> options)
    {
        _storage = storage;
        _options = options.Value;
    }

    [HttpPost("verify")]
    public async Task<ActionResult<StorageVerificationResponse>> Verify(
        CancellationToken cancellationToken)
    {
        var objectKey = $"healthchecks/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}.txt";
        var payload = Encoding.UTF8.GetBytes("HanYu Backblaze B2 storage verification");

        try
        {
            await using var stream = new MemoryStream(payload, writable: false);
            var uploaded = await _storage.UploadAsync(
                objectKey,
                stream,
                "text/plain",
                cancellationToken);

            await _storage.DeleteAsync(uploaded.ObjectKey, cancellationToken);

            return Ok(new StorageVerificationResponse(
                true,
                _options.Provider,
                _options.ServiceUrl,
                _options.Region,
                _options.PublicBucketName,
                uploaded.ObjectKey,
                uploaded.PublicUrl,
                "Kết nối storage thành công: upload và delete đều hoạt động."));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ProblemDetails
                {
                    Title = "Không thể kết nối storage",
                    Detail = ex.Message,
                    Status = StatusCodes.Status503ServiceUnavailable
                });
        }
    }
}

public sealed record StorageVerificationResponse(
    bool Success,
    string Provider,
    string? ServiceUrl,
    string Region,
    string BucketName,
    string ObjectKey,
    string ReadUrl,
    string Message);
