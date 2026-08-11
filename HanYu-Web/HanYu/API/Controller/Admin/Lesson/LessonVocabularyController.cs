using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Vocabulary;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/lessons/{lessonId:long}/vocabulary")]
public sealed class LessonVocabularyController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private const string ContentEditRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor;

    private readonly ILessonAdminService _service;

    public LessonVocabularyController(ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(long lessonId, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetVocabularyAsync(lessonId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Attach(
        long lessonId,
        AttachLessonVocabularyRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.AttachVocabularyAsync(lessonId, request, cancellationToken));

    [HttpPut("{vocabularyId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Update(
        long lessonId,
        long vocabularyId,
        UpdateLessonVocabularyRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.UpdateVocabularyAsync(lessonId, vocabularyId, request, cancellationToken));

    [HttpDelete("{vocabularyId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Delete(
        long lessonId,
        long vocabularyId,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.DetachVocabularyAsync(lessonId, vocabularyId, cancellationToken));
}
