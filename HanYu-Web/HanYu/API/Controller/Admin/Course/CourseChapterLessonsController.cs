using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin.Chapters.Lessons;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/courses/{courseId:long}/chapters/{chapterId:long}/lessons")]
public sealed class CourseChapterLessonsController : ControllerBase
{
    private readonly IAdminCourseService _service;
    private readonly ICourseCurriculumReorderService _reorderService;

    public CourseChapterLessonsController(
        IAdminCourseService service,
        ICourseCurriculumReorderService reorderService)
    {
        _service = service;
        _reorderService = reorderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long courseId,
        long chapterId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetChapterLessonsAsync(courseId, chapterId, cancellationToken));

    [HttpPost("assign")]
    public async Task<IActionResult> Assign(
        long courseId,
        long chapterId,
        [FromBody] AssignLessonToChapterRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.AssignLessonToChapterAsync(
                courseId, chapterId, request, cancellationToken));

    [HttpPost("{lessonId:long}/move")]
    public async Task<IActionResult> Move(
        long courseId,
        long chapterId,
        long lessonId,
        [FromBody] MoveLessonToChapterRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.MoveLessonAsync(
                courseId, chapterId, lessonId, request, cancellationToken));

    [HttpDelete("{lessonId:long}")]
    public async Task<IActionResult> Remove(
        long courseId,
        long chapterId,
        long lessonId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RemoveLessonFromChapterAsync(
                courseId, chapterId, lessonId, cancellationToken));

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(
        long courseId,
        long chapterId,
        [FromBody] ReorderChapterLessonsRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _reorderService.ReorderChapterLessonsAsync(
                courseId, chapterId, request, cancellationToken));
}
