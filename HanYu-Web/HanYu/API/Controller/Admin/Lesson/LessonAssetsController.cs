using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Lesson.Admin.Assets;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Lesson;

[ApiController]
[Authorize(Roles = ContentReadRoles)]
[Route("api/v1/admin/lessons/{lessonId:long}/assets")]
public sealed class LessonAssetsController : ControllerBase
{
    private const string ContentReadRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor + "," + Roles.Reviewer;

    private const string ContentEditRoles =
        Roles.SuperAdmin + "," + Roles.Admin + "," + Roles.ContentManager + "," + Roles.ContentEditor;

    private readonly ILessonAdminService _service;

    public LessonAssetsController(ILessonAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(long lessonId, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetAssetsAsync(lessonId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Create(
        long lessonId,
        CreateLessonAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.CreateAssetAsync(lessonId, request, cancellationToken));

    [HttpPut("{assetId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Update(
        long lessonId,
        long assetId,
        UpdateLessonAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.UpdateAssetAsync(lessonId, assetId, request, cancellationToken));

    [HttpDelete("{assetId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    public async Task<IActionResult> Delete(
        long lessonId,
        long assetId,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.DeleteAssetAsync(lessonId, assetId, cancellationToken));
}
