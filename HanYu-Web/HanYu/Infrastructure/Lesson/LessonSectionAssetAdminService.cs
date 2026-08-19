using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Admin.SectionAssets;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Lesson;

public sealed class LessonSectionAssetAdminService
{
    private readonly IHanYuDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LessonSectionAssetAdminService(IHanYuDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyCollection<AdminLessonSectionAssetResponse>>> GetAsync(
        long lessonId,
        long sectionId,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateSectionAsync(lessonId, sectionId, cancellationToken);
        if (!validation.IsSuccess)
            return Result.Failure<IReadOnlyCollection<AdminLessonSectionAssetResponse>>(validation.Error);

        var entities = await _db.Set<LessonSectionAsset>()
            .AsNoTracking()
            .Include(x => x.LessonAsset)
                .ThenInclude(x => x.AudioAsset)
            .Where(x => x.LessonSectionId == sectionId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<AdminLessonSectionAssetResponse>>(
            entities.Select(x => Map(x, x.LessonAsset)).ToArray());
    }

    public async Task<Result<AdminLessonSectionAssetResponse>> AttachAsync(
        long lessonId,
        long sectionId,
        AttachLessonSectionAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateSectionAsync(lessonId, sectionId, cancellationToken);
        if (!validation.IsSuccess)
            return Result.Failure<AdminLessonSectionAssetResponse>(validation.Error);

        var asset = await _db.Set<LessonAsset>()
            .Include(x => x.AudioAsset)
            .FirstOrDefaultAsync(x => x.Id == request.LessonAssetId && x.LessonId == lessonId, cancellationToken);
        if (asset is null)
            return Result.Failure<AdminLessonSectionAssetResponse>(Error.NotFound("LessonSectionAsset.AssetNotFound", "Không tìm thấy tài nguyên thuộc Lesson này."));

        var duplicate = await _db.Set<LessonSectionAsset>()
            .AnyAsync(x => x.LessonSectionId == sectionId && x.LessonAssetId == request.LessonAssetId, cancellationToken);
        if (duplicate)
            return Result.Failure<AdminLessonSectionAssetResponse>(Error.Conflict("LessonSectionAsset.Duplicate", "Tài nguyên đã được gắn vào section này."));

        LessonSectionAsset link;
        try
        {
            link = new LessonSectionAsset(sectionId, request.LessonAssetId, request.SortOrder, request.CaptionVi, request.IsRequired);
            if (_currentUser.UserId is Guid userId && userId != Guid.Empty)
                link.SetCreatedBy(userId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<AdminLessonSectionAssetResponse>(Error.Validation("LessonSectionAsset.ValidationFailed", ex.Message));
        }

        _db.Add(link);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(link, asset));
    }

    public async Task<Result<AdminLessonSectionAssetResponse>> UpdateAsync(
        long lessonId,
        long sectionId,
        long linkId,
        UpdateLessonSectionAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateSectionAsync(lessonId, sectionId, cancellationToken);
        if (!validation.IsSuccess)
            return Result.Failure<AdminLessonSectionAssetResponse>(validation.Error);

        var link = await _db.Set<LessonSectionAsset>()
            .Include(x => x.LessonAsset)
                .ThenInclude(x => x.AudioAsset)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.LessonSectionId == sectionId, cancellationToken);
        if (link is null)
            return Result.Failure<AdminLessonSectionAssetResponse>(Error.NotFound("LessonSectionAsset.NotFound", "Không tìm thấy liên kết tài nguyên của section."));

        try
        {
            link.Update(request.SortOrder, request.CaptionVi, request.IsRequired);
            if (_currentUser.UserId is Guid userId && userId != Guid.Empty)
                link.MarkAsUpdated(userId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<AdminLessonSectionAssetResponse>(Error.Validation("LessonSectionAsset.ValidationFailed", ex.Message));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success(Map(link, link.LessonAsset));
    }

    public async Task<Result> DeleteAsync(
        long lessonId,
        long sectionId,
        long linkId,
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateSectionAsync(lessonId, sectionId, cancellationToken);
        if (!validation.IsSuccess) return Result.Failure(validation.Error);

        var link = await _db.Set<LessonSectionAsset>()
            .FirstOrDefaultAsync(x => x.Id == linkId && x.LessonSectionId == sectionId, cancellationToken);
        if (link is null)
            return Result.Failure(Error.NotFound("LessonSectionAsset.NotFound", "Không tìm thấy liên kết tài nguyên của section."));

        _db.Remove(link);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> ValidateSectionAsync(long lessonId, long sectionId, CancellationToken cancellationToken)
    {
        if (lessonId <= 0 || sectionId <= 0)
            return Result.Failure(Error.Validation("LessonSectionAsset.InvalidId", "Lesson ID hoặc Section ID không hợp lệ."));

        var exists = await _db.Set<LessonSection>()
            .AnyAsync(x => x.Id == sectionId && x.LessonId == lessonId, cancellationToken);
        return exists
            ? Result.Success()
            : Result.Failure(Error.NotFound("LessonSectionAsset.SectionNotFound", "Không tìm thấy section thuộc Lesson này."));
    }

    private static AdminLessonSectionAssetResponse Map(LessonSectionAsset x, LessonAsset? asset = null)
    {
        asset ??= x.LessonAsset;
        var url = asset.AudioAsset?.PublicUrl ?? asset.Url;

        return new AdminLessonSectionAssetResponse(
            x.Id,
            x.PublicId,
            x.LessonSectionId,
            x.LessonAssetId,
            x.SortOrder,
            x.CaptionVi,
            x.IsRequired,
            asset.AssetType.ToString(),
            url,
            asset.AudioAssetId,
            asset.CaptionVi,
            x.CreatedAt,
            x.UpdatedAt);
    }
}
