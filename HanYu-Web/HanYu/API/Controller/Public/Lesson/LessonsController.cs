using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Public.Lessons;
using HanYu.Application.Interfaces.Lesson;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Lesson;

[ApiController]
[Route("api/v1/public/lessons")]
public sealed class LessonsController : ControllerBase
{
    private readonly ILessonPublicService _service;

    public LessonsController(
        ILessonPublicService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] LessonQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetLessonsAsync(
                query,
                cancellationToken));

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> Get(
        Guid publicId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetLessonAsync(
                userId: null,
                lessonPublicId: publicId,
                cancellationToken));

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await ResolveBySlugAsync(
                slug,
                cancellationToken));

    [HttpGet("{slug}/content")]
    public async Task<IActionResult> GetContentBySlug(
        string slug,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await ResolveBySlugAsync(
                slug,
                cancellationToken));

    private async Task<Result<LessonDetailResponse>> ResolveBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<LessonDetailResponse>(
                Error.Validation(
                    "Lesson.InvalidSlug",
                    "Slug Lesson không hợp lệ."));
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var lookup = await _service.GetLessonsAsync(
            new LessonQuery
            {
                Q = normalizedSlug,
                Page = 1,
                PageSize = 100,
                Sort = "sortOrder"
            },
            cancellationToken);

        if (lookup.IsFailure)
        {
            return Result.Failure<LessonDetailResponse>(
                lookup.Error);
        }

        var lesson = lookup.Value.Items
            .FirstOrDefault(
                item => string.Equals(
                    item.Slug,
                    normalizedSlug,
                    StringComparison.OrdinalIgnoreCase));

        if (lesson is null)
        {
            return Result.Failure<LessonDetailResponse>(
                Error.NotFound(
                    "Lesson.NotFound",
                    "Không tìm thấy Lesson đã xuất bản."));
        }

        return await _service.GetLessonAsync(
            userId: null,
            lessonPublicId: lesson.PublicId,
            cancellationToken);
    }
}
