using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Lesson;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Lesson;

[ApiController]
[Route("api/v1/public/lessons")]
public sealed class LessonsController : ControllerBase
{
    private readonly IPublicLessonService _service;

    public LessonsController(IPublicLessonService service)
    {
        _service = service;
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> Get(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.GetAccessibleLessonAsync(publicId, cancellationToken));
    }

    [HttpPost("{publicId:guid}/start")]
    public async Task<IActionResult> Start(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.StartAsync(publicId, cancellationToken));
    }

    [HttpPost("{publicId:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid publicId,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.CompleteAsync(publicId, idempotencyKey, cancellationToken));
    }
}
