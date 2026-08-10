using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Public;
using HanYu.Application.Interfaces.Course;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Course;

[ApiController]
[Route("api/v1/public/courses")]
public sealed class CoursesController : ControllerBase
{
    private readonly IPublicCourseService _service;

    public CoursesController(
        IPublicCourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PublicCourseQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetPublishedCoursesAsync(
                query,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> Get(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetCourseAsync(
                publicId,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetCourseBySlugAsync(
                slug,
                cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("{publicId:guid}/curriculum")]
    public async Task<IActionResult> GetCurriculum(
        Guid publicId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetCurriculumAsync(
                publicId,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
