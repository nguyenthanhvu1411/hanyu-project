using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Sections;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route(
    "api/v1/admin/lessons/{lessonId:long}/sections")]
public sealed class LessonSectionsController
    : ControllerBase
{
    private readonly ILessonAdminService _service;

    public LessonSectionsController(
        ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        long lessonId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetSectionsAsync(
                lessonId,
                cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        long lessonId,
        CreateLessonSectionRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.CreateSectionAsync(
                lessonId,
                request,
                cancellationToken));

    [HttpPut("{sectionId:long}")]
    public async Task<IActionResult> Update(
        long lessonId,
        long sectionId,
        UpdateLessonSectionRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.UpdateSectionAsync(
                lessonId,
                sectionId,
                request,
                cancellationToken));

    [HttpDelete("{sectionId:long}")]
    public async Task<IActionResult> Delete(
        long lessonId,
        long sectionId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.DeleteSectionAsync(
                lessonId,
                sectionId,
                cancellationToken));
}
