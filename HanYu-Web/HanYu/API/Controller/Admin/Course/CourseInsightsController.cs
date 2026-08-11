using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Course.Admin.Insights;
using HanYu.Application.Interfaces.Course;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Course;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/courses/{courseId:long}")]
public sealed class CourseInsightsController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private readonly ICourseAdminInsightsService _service;

    public CourseInsightsController(ICourseAdminInsightsService service)
    {
        _service = service;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        long courseId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetHistoryAsync(courseId, cancellationToken));

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(
        long courseId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetStatisticsAsync(courseId, cancellationToken));

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
        long courseId,
        [FromQuery] CourseStudentsQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetStudentsAsync(courseId, query, cancellationToken));
}
