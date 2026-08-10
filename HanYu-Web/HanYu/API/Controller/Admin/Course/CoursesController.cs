using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/courses")]
public sealed class CoursesController : ControllerBase
{
    private readonly IAdminCourseService _service;
    private readonly ICourseCurriculumReorderService _reorderService;

    public CoursesController(
        IAdminCourseService service,
        ICourseCurriculumReorderService reorderService)
    {
        _service = service;
        _reorderService = reorderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminCourseQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetCoursesAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Code = "Course.InvalidId",
                Message = "Course ID không hợp lệ."
            });
        }

        return this.ToActionResult(
            await _service.GetCourseAsync(id, cancellationToken));
    }

    [HttpPost]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateCourseAsync(request, cancellationToken));

    [HttpPut("{id:long}")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateCourseAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/validate")]
    public async Task<IActionResult> Validate(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ValidateCourseAsync(id, cancellationToken));

    [HttpPost("{id:long}/submit-review")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.SubmitForReviewAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/approve")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ApproveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/reject")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Reject(
        long id,
        RejectCourseRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RejectAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/publish")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.PublishAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/schedule-publish")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SchedulePublish(
        long id,
        ScheduleCoursePublishRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.SchedulePublishAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/archive")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Archive(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreAsync(id, request, cancellationToken));

    [HttpDelete("{id:long}")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore-deleted")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> RestoreDeleted(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreDeletedAsync(id, request, cancellationToken));

    [HttpPut("{id:long}/chapters/order")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> ReorderChapters(
        long id,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _reorderService.ReorderChaptersAsync(id, request, cancellationToken));
}
