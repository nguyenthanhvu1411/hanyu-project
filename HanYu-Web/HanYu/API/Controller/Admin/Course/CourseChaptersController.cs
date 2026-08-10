using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Features.Course.Admin.Chapters;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/courses/{courseId:long}/chapters")]
public sealed class CourseChaptersController
    : ControllerBase
{
    private readonly IAdminCourseService _service;

    public CourseChaptersController(
        IAdminCourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long courseId,
        [FromQuery] bool includeDeleted,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetChaptersAsync(
                courseId,
                includeDeleted,
                cancellationToken));

    [HttpGet("{chapterId:long}")]
    public async Task<IActionResult> Get(
        long courseId,
        long chapterId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetChapterAsync(
                courseId,
                chapterId,
                cancellationToken));

    [HttpPost]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        long courseId,
        CreateCourseChapterRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateChapterAsync(
                courseId,
                request,
                cancellationToken));

    [HttpPut("{chapterId:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long courseId,
        long chapterId,
        UpdateCourseChapterRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateChapterAsync(
                courseId,
                chapterId,
                request,
                cancellationToken));

    [HttpDelete("{chapterId:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long courseId,
        long chapterId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteChapterAsync(
                courseId,
                chapterId,
                request,
                cancellationToken));

    [HttpPost("{chapterId:long}/restore")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long courseId,
        long chapterId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreChapterAsync(
                courseId,
                chapterId,
                request,
                cancellationToken));
}
