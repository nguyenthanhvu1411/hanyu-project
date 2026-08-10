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

    public CoursesController(
        IAdminCourseService service)
    {
        _service = service;
    }

    // =========================================================
    // LIST
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminCourseQuery query,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.GetCoursesAsync(
                query,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // DETAIL
    // =========================================================

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(
                new
                {
                    Code = "Course.InvalidId",
                    Message = "Course ID không hợp lệ."
                });
        }

        var result =
            await _service.GetCourseAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // CREATE
    // =========================================================

    [HttpPost]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.CreateCourseAsync(
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    [HttpPut("{id:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.UpdateCourseAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // VALIDATE
    // =========================================================

    [HttpPost("{id:long}/validate")]
    public async Task<IActionResult> Validate(
        long id,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ValidateCourseAsync(
                id,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // SUBMIT REVIEW
    // =========================================================

    [HttpPost("{id:long}/submit-review")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.SubmitForReviewAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // APPROVE
    // =========================================================

    [HttpPost("{id:long}/approve")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ApproveAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // REJECT
    // =========================================================

    [HttpPost("{id:long}/reject")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Reject(
        long id,
        RejectCourseRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RejectAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // PUBLISH
    // =========================================================

    [HttpPost("{id:long}/publish")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.PublishAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // SCHEDULE PUBLISH
    // =========================================================

    [HttpPost("{id:long}/schedule-publish")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SchedulePublish(
        long id,
        ScheduleCoursePublishRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.SchedulePublishAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // ARCHIVE
    // =========================================================

    [HttpPost("{id:long}/archive")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Archive(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ArchiveAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // RESTORE ARCHIVED
    // =========================================================

    [HttpPost("{id:long}/restore")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RestoreAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // DELETE
    // =========================================================

    [HttpDelete("{id:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.DeleteAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // RESTORE DELETED
    // =========================================================

    [HttpPost("{id:long}/restore-deleted")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> RestoreDeleted(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.RestoreDeletedAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }

    // =========================================================
    // REORDER CHAPTERS
    // =========================================================

    [HttpPut("{id:long}/chapters/order")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> ReorderChapters(
        long id,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken)
    {
        var result =
            await _service.ReorderChaptersAsync(
                id,
                request,
                cancellationToken);

        return this.ToActionResult(result);
    }
}
