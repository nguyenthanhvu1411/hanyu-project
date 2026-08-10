using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Prerequisites;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/lessons/{lessonId:long}/prerequisites")]
public sealed class LessonPrerequisitesController
    : ControllerBase
{
    private readonly ILessonAdminService _service;

    public LessonPrerequisitesController(
        ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long lessonId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetPrerequisitesAsync(
                lessonId,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Add(
        long lessonId,
        AddLessonPrerequisiteRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.AddPrerequisiteAsync(
                lessonId,
                request,
                cancellationToken));

    [HttpDelete("{requiredLessonId:long}")]
    public async Task<IActionResult> Delete(
        long lessonId,
        long requiredLessonId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.RemovePrerequisiteAsync(
                lessonId,
                requiredLessonId,
                cancellationToken));
}
