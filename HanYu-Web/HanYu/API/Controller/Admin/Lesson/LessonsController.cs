using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin/lessons")]
public sealed class LessonsController : ControllerBase
{
    private readonly ILessonAdminService _service;

    public LessonsController(ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] AdminLessonQuery query,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.GetLessonsAsync(query, cancellationToken));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(
        long id,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.GetLessonAsync(id, cancellationToken));
    }

    [HttpPost]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        CreateLessonRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.CreateLessonAsync(request, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.UpdateLessonAsync(id, request, cancellationToken));
    }

    [HttpGet("{id:long}/validate")]
    public async Task<IActionResult> Validate(
        long id,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.ValidateLessonAsync(id, cancellationToken));
    }

    [HttpPost("{id:long}/submit-review")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> SubmitReview(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.SubmitForReviewAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/approve")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Approve(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.ApproveAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/publish")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Publish(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.PublishAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/archive")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Archive(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.ArchiveAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/restore")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.RestoreAsync(id, request, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.DeleteAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:long}/restore-deleted")]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> RestoreDeleted(
        long id,
        [FromBody] LessonWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        return this.ToActionResult(
            await _service.RestoreDeletedAsync(id, request, cancellationToken));
    }
}
