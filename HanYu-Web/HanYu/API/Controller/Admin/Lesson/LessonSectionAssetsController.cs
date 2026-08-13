using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.SectionAssets;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Constants;
using HanYu.Infrastructure.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/lessons/{lessonId:long}/sections/{sectionId:long}/assets")]
public sealed class LessonSectionAssetsController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private const string ContentEditRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor;

    private readonly LessonSectionAssetAdminService _service;

    public LessonSectionAssetsController(IHanYuDbContext dbContext, ICurrentUserService currentUser)
    {
        _service = new LessonSectionAssetAdminService(dbContext, currentUser);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(long lessonId, long sectionId, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetAsync(lessonId, sectionId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Attach(
        long lessonId,
        long sectionId,
        AttachLessonSectionAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.AttachAsync(lessonId, sectionId, request, cancellationToken));

    [HttpPut("{linkId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Update(
        long lessonId,
        long sectionId,
        long linkId,
        UpdateLessonSectionAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.UpdateAsync(lessonId, sectionId, linkId, request, cancellationToken));

    [HttpDelete("{linkId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Delete(
        long lessonId,
        long sectionId,
        long linkId,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.DeleteAsync(lessonId, sectionId, linkId, cancellationToken));
}
