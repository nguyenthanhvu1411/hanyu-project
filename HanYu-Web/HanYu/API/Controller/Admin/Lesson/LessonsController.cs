using HanYu.API.Common;
using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/lessons")]
public sealed class LessonsController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private const string ContentEditRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor;

    private const string ContentReviewRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.Reviewer;

    private const string ContentPublishRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager;

    private readonly ILessonAdminService _service;
    private readonly IHanYuDbContext _dbContext;

    public LessonsController(
        ILessonAdminService service,
        IHanYuDbContext dbContext)
    {
        _service = service;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLessonQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetLessonsAsync(query, cancellationToken));

    [HttpGet("slug-availability")]
    public async Task<IActionResult> GetSlugAvailability(
        [FromQuery] string slug,
        [FromQuery] long? excludeId,
        CancellationToken cancellationToken)
    {
        var normalized = SlugAvailabilityQueries.Normalize(slug);
        var available = await SlugAvailabilityQueries.IsLessonSlugAvailableAsync(
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
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetLessonAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsLessonSlugAvailableAsync(
                _dbContext,
                slug,
                cancellationToken: cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminLessonDetailDto>(
                    Error.Conflict(
                        "Lesson.SlugAlreadyExists",
                        "Slug bài giảng đã tồn tại.")));
        }

        return this.ToActionResult(
            await _service.CreateLessonAsync(request, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        var slug = SlugAvailabilityQueries.Normalize(request.Slug);
        if (slug.Length > 0 &&
            !await SlugAvailabilityQueries.IsLessonSlugAvailableAsync(
                _dbContext,
                slug,
                id,
                cancellationToken))
        {
            return this.ToActionResult(
                Result.Failure<AdminLessonDetailDto>(
                    Error.Conflict(
                        "Lesson.SlugAlreadyExists",
                        "Slug bài giảng đã được bài giảng khác sử dụng.")));
        }

        return this.ToActionResult(
            await _service.UpdateLessonAsync(id, request, cancellationToken));
    }

    [HttpGet("{id:long}/validate")]
    public async Task<IActionResult> Validate(
        long id,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await LessonWorkflowValidator.ValidateAsync(
                _dbContext,
                id,
                LessonWorkflowValidationTarget.General,
                cancellationToken));

    [HttpPost("{id:long}/submit-review")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await LessonWorkflowValidator.ValidateAsync(
            _dbContext,
            id,
            LessonWorkflowValidationTarget.SubmitReview,
            cancellationToken);

        if (validation.IsFailure)
        {
            return this.ToActionResult(validation);
        }

        if (!validation.Value.IsValid)
        {
            return this.ToActionResult(
                Result.Failure<AdminLessonDetailDto>(
                    BuildWorkflowValidationError(
                        "Lesson.NotReviewable",
                        "Lesson chưa đủ điều kiện để gửi duyệt.",
                        validation.Value)));
        }

        return this.ToActionResult(
            await _service.SubmitForReviewAsync(
                id,
                request,
                cancellationToken));
    }

    [HttpPost("{id:long}/approve")]
    [Authorize(Roles = ContentReviewRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await LessonWorkflowValidator.ValidateAsync(
            _dbContext,
            id,
            LessonWorkflowValidationTarget.General,
            cancellationToken);

        if (validation.IsFailure)
        {
            return this.ToActionResult(validation);
        }

        if (!validation.Value.IsValid)
        {
            return this.ToActionResult(
                Result.Failure<AdminLessonDetailDto>(
                    BuildWorkflowValidationError(
                        "Lesson.NotApprovable",
                        "Lesson còn lỗi validation và chưa thể được duyệt.",
                        validation.Value)));
        }

        return this.ToActionResult(
            await _service.ApproveAsync(
                id,
                request,
                cancellationToken));
    }

    [HttpPost("{id:long}/publish")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await LessonWorkflowValidator.ValidateAsync(
            _dbContext,
            id,
            LessonWorkflowValidationTarget.Publish,
            cancellationToken);

        if (validation.IsFailure)
        {
            return this.ToActionResult(validation);
        }

        if (!validation.Value.IsValid)
        {
            return this.ToActionResult(
                Result.Failure<AdminLessonDetailDto>(
                    BuildWorkflowValidationError(
                        "Lesson.NotPublishable",
                        "Lesson chưa đủ điều kiện để Publish.",
                        validation.Value)));
        }

        return this.ToActionResult(
            await _service.PublishAsync(
                id,
                request,
                cancellationToken));
    }

    [HttpPost("{id:long}/archive")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Archive(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.ArchiveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.RestoreAsync(id, request, cancellationToken));

    [HttpDelete("{id:long}")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.DeleteAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/restore-deleted")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> RestoreDeleted(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.RestoreDeletedAsync(id, request, cancellationToken));

    private static Error BuildWorkflowValidationError(
        string code,
        string prefix,
        LessonValidationResultDto validation)
    {
        var errors = validation.Issues
            .Where(issue => issue.Severity == LessonValidationSeverity.Error)
            .Select(issue => issue.Message)
            .ToArray();

        var message = errors.Length == 0
            ? prefix
            : $"{prefix} {string.Join(" | ", errors)}";

        return Error.Validation(
            code,
            message);
    }
}
