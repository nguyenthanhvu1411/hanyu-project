using HanYu.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Media;

[ApiController]
[Route("api/v1/media")]
public sealed class MediaController : ControllerBase
{
    private readonly IPublicFileStorage _storage;

    public MediaController(IPublicFileStorage storage)
    {
        _storage = storage;
    }

    [HttpGet("read-url")]
    public async Task<IActionResult> GetReadUrl(
        [FromQuery] string objectKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Object key không hợp lệ",
                Detail = "objectKey không được để trống.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var normalized = objectKey.Trim().TrimStart('/');
        if (normalized.Contains("..", StringComparison.Ordinal) || normalized.StartsWith('.'))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Object key không hợp lệ",
                Detail = "objectKey chứa ký tự đường dẫn không hợp lệ.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var url = await _storage.GetReadUrlAsync(normalized, cancellationToken);
        return Ok(new MediaReadUrlResponse(normalized, url));
    }
}

public sealed record MediaReadUrlResponse(string ObjectKey, string Url);
