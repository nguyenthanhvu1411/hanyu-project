using HanYu.API.Common;
using HanYu.API.Common.Extensions;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Admin.Assets;
using HanYu.Application.Interfaces.Lesson;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Lesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

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
    private readonly IHanYuDbContext _dbContext;

    public LessonAssetsController(
        ILessonAdminService service,
        IHanYuDbContext dbContext)
    {
        _service = service;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(long lessonId, CancellationToken cancellationToken)
        => this.ToActionResult(await _service.GetAssetsAsync(lessonId, cancellationToken));

    [HttpPost]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Create(
        long lessonId,
        CreateLessonAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.CreateAssetAsync(lessonId, request, cancellationToken));

    [HttpPut("{assetId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Update(
        long lessonId,
        long assetId,
        UpdateLessonAssetRequest request,
        CancellationToken cancellationToken)
        => this.ToActionResult(await _service.UpdateAssetAsync(lessonId, assetId, request, cancellationToken));

    [HttpDelete("{assetId:long}")]
    [Authorize(Roles = ContentEditRoles)]
    [EnableRateLimiting(ApiFoundationExtensions.AdminWriteRateLimitPolicy)]
    public async Task<IActionResult> Delete(
        long lessonId,
        long assetId,
        CancellationToken cancellationToken)
    {
        var linkedToSection = await _dbContext.Set<LessonSectionAsset>()
            .AsNoTracking()
            .AnyAsync(
                x => x.LessonAssetId == assetId &&
                     x.LessonAsset.LessonId == lessonId,
                cancellationToken);

        if (linkedToSection)
        {
            return this.ToActionResult(
                Result.Failure(
                    Error.Conflict(
                        "LessonAsset.InUse",
                        "Tài nguyên đang được sử dụng trong một hoặc nhiều section. Hãy gỡ media khỏi các section trước khi xóa tài nguyên.")));
        }

        return this.ToActionResult(
            await _service.DeleteAssetAsync(
                lessonId,
                assetId,
                cancellationToken));
    }
}
