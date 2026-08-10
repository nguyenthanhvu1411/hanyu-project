using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin.Chapters.Lessons;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/courses/{courseId:long}/chapters/{chapterId:long}/lessons")]
public sealed class CourseChapterLessonsController
    : ControllerBase
{
    private readonly IAdminCourseService _service;

    public CourseChapterLessonsController(
        IAdminCourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long courseId,
        long chapterId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetChapterLessonsAsync(
                courseId,
                chapterId,
                cancellationToken);

        return this.ToActionResult(
            result);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign(
        long courseId,
        long chapterId,
        [FromBody] AssignLessonToChapterRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.AssignLessonToChapterAsync(
                courseId,
                chapterId,
                request,
                cancellationToken);

        return this.ToActionResult(
            result);
    }

    [HttpPost("{lessonId:long}/move")]
    public async Task<IActionResult> Move(
        long courseId,
        long chapterId,
        long lessonId,
        [FromBody] MoveLessonToChapterRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.MoveLessonAsync(
                courseId,
                chapterId,
                lessonId,
                request,
                cancellationToken);

        return this.ToActionResult(
            result);
    }

    [HttpDelete("{lessonId:long}")]
    public async Task<IActionResult> Remove(
        long courseId,
        long chapterId,
        long lessonId,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RemoveLessonFromChapterAsync(
                courseId,
                chapterId,
                lessonId,
                cancellationToken);

        return this.ToActionResult(
            result);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(
        long courseId,
        long chapterId,
        [FromBody] ReorderChapterLessonsRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ReorderChapterLessonsAsync(
                courseId,
                chapterId,
                request,
                cancellationToken);

        return this.ToActionResult(
            result);
    }
}
