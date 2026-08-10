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
            await _service.GetAccessibleLessonAsync(
                publicId,
                cancellationToken));
    }
}
