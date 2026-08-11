using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Interfaces.Lesson;
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

    public LessonsController(ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLessonQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetLessonsAsync(query, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetLessonAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateLessonRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.CreateLessonAsync(request, cancellationToken));

    [HttpPut("{id:long}")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.UpdateLessonAsync(id, request, cancellationToken));

    [HttpGet("{id:long}/validate")]
    public async Task<IActionResult> Validate(long id, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.ValidateLessonAsync(id, cancellationToken));

    [HttpPost("{id:long}/submit-review")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.SubmitForReviewAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/approve")]
    [Authorize(Roles = ContentReviewRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.ApproveAsync(id, request, cancellationToken));

    [HttpPost("{id:long}/publish")]
    [Authorize(Roles = ContentPublishRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.PublishAsync(id, request, cancellationToken));

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
}
