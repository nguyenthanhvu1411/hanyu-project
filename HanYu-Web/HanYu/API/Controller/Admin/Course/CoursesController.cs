using HanYu.API.Common;
using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Interfaces.Course;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/courses")]
public sealed class CoursesController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private const string ContentEditRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor;

    private const string ContentReviewRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.Reviewer;

    private const string ContentPublishRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager;

    private readonly IAdminCourseService _service;
    private readonly ICourseCurriculumReorderService _reorderService;
    private readonly IHanYuDbContext _dbContext;

    public CoursesController(
        IAdminCourseService service,
        ICourseCurriculumReorderService reorderService,
        IHanYuDbContext dbContext)
    {
        _service = service;
        _reorderService = reorderService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminCourseQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetCoursesAsync(query, cancellationToken));

    [HttpGet("slug-availability")]
    public async Task<IActionResult> GetSlugAvailability(
        [FromQuery] string slug,
        [FromQuery] long? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = SlugAvailabilityQueries.Normalize(slug);
        var available = await SlugAvailabilityQueries.IsCourseSlugAvailableAsync(
            _dbContext,
            normalized,
            excludeId,
            cancellationToken);

        return Ok(new
        {
            Slug = normalized,
            Available = available
        });
    }

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
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsCourseSlugAvailableAsync(
                _dbContext,
                slug,
                cancellationToken: cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminCourseDetailDto>(
                    Error.Conflict(
                        "Course.SlugAlreadyExists",
                        "Slug khóa học đã tồn tại.")));
        }

        return this.ToActionResult(
            await _service.CreateCourseAsync(request, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsCourseSlugAvailableAsync(
                _dbContext,
                slug,
                id,
                cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminCourseDetailDto>(
                    Error.Conflict(
                        "Course.SlugAlreadyExists",
                        "Slug khóa học đã tồn tại ở một khóa học khác.")));
        }

        return this.ToActionResult(
            await _service.UpdateCourseAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/validate")]
    public async Task<IActionResult> Validate(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ValidateCourseAsync(id, cancellationToken));

    [HttpPost("{id:long}/submit-review")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.SubmitForReviewAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/approve")]
    [Authorize(Roles = ContentReviewRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ApproveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/reject")]
    [Authorize(Roles = ContentReviewRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Reject(
        long id,
        RejectCourseRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RejectAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/publish")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.PublishAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/archive")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Archive(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.ArchiveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreAsync(id, request, cancellationToken));

    [HttpDelete("{id:long}")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore-deleted")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> RestoreDeleted(
        long id,
        CourseWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestoreDeletedAsync(id, request, cancellationToken));

    [HttpPut("{id:long}/chapters/order")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> ReorderChapters(
        long id,
        ReorderCourseChaptersRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _reorderService.ReorderChaptersAsync(id, request, cancellationToken));
}