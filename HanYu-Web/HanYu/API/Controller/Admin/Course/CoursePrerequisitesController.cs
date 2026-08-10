using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin;
using HanYu.Application.Features.Course.Admin.Prerequisites;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/courses/{courseId:long}/prerequisites")]
public sealed class CoursePrerequisitesController
    : ControllerBase
{
    private readonly IAdminCourseService _service;

    public CoursePrerequisitesController(
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
            await _service.GetPrerequisitesAsync(
                courseId,
                includeDeleted,
                cancellationToken));

    [HttpGet("{prerequisiteId:long}")]
    public async Task<IActionResult> Get(
        long courseId,
        long prerequisiteId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetPrerequisiteAsync(
                courseId,
                prerequisiteId,
                cancellationToken));

    [HttpPost]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        long courseId,
        CreateCoursePrerequisiteRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreatePrerequisiteAsync(
                courseId,
                request,
                cancellationToken));

    [HttpPut("{prerequisiteId:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long courseId,
        long prerequisiteId,
        UpdateCoursePrerequisiteRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdatePrerequisiteAsync(
                courseId,
                prerequisiteId,
                request,
                cancellationToken));

    [HttpDelete("{prerequisiteId:long}")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long courseId,
        long prerequisiteId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeletePrerequisiteAsync(
                courseId,
                prerequisiteId,
                request,
                cancellationToken));

    [HttpPost("{prerequisiteId:long}/restore")]
    [EnableRateLimiting(
        ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Restore(
        long courseId,
        long prerequisiteId,
        CourseEntityWorkflowRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RestorePrerequisiteAsync(
                courseId,
                prerequisiteId,
                request,
                cancellationToken));
}
