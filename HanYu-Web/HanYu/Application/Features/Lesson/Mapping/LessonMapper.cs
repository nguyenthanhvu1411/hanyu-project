using HanYu.Application.Features.Lesson.Public.Bookmarks;
using HanYu.Application.Features.Lesson.Public.Lessons;
using HanYu.Application.Features.Lesson.Public.Progress;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Lesson;

namespace HanYu.Application.Features.Lesson.Mapping;

public static class LessonMapper
{
    public static LessonListItemResponse ToListItem(
        Domain.Entities.Lesson.Lesson entity)
    {
        return new LessonListItemResponse(
            entity.PublicId,
            entity.Slug,
            entity.TitleVi,
            entity.ShortDescriptionVi,
            entity.CoverImageUrl,
            entity.HskLevelId,
            entity.HskLevel.Code,
            entity.EstimatedMinutes,
            entity.Difficulty,
            entity.IsFeatured,
            entity.Topic?.Slug,
            entity.Topic?.NameVi);
    }

    public static LessonSectionResponse ToSection(
        LessonSection entity,
        IReadOnlyCollection<LessonSectionMediaResponse>? media = null)
    {
        return new LessonSectionResponse(
            entity.PublicId,
            entity.SectionType,
            entity.TitleVi,
            entity.ContentVi,
            entity.SortOrder,
            entity.IsRequired,
            entity.EstimatedSeconds,
            media ?? Array.Empty<LessonSectionMediaResponse>());
    }

    public static LessonAssetResponse ToAsset(
        LessonAsset entity)
    {
        var url =
            entity.AudioAsset?.PublicUrl
            ?? entity.Url;

        return new LessonAssetResponse(
            entity.PublicId,
            entity.AssetType,
            url,
            entity.CaptionVi,
            entity.SortOrder);
    }

    public static LessonVocabularyResponse ToVocabulary(
        LessonVocabulary entity)
    {
        return new LessonVocabularyResponse(
            entity.Vocabulary.PublicId,
            entity.Vocabulary.Simplified,
            entity.Vocabulary.Traditional,
            entity.Vocabulary.Pinyin,
            entity.Vocabulary.PrimaryMeaningVi,
            entity.SortOrder,
            entity.IsRequired);
    }

    public static LessonProgressResponse ToProgress(
        UserLessonProgress entity,
        Guid? lastSectionPublicId)
    {
        return new LessonProgressResponse(
            entity.Status,
            lastSectionPublicId,
            entity.LastPosition,
            entity.CompletionPercent,
            entity.StartedAt,
            entity.LastAccessedAt,
            entity.CompletedAt);
    }

    public static LessonSectionProgressResponse ToSectionProgress(
        UserLessonSectionProgress entity,
        Guid publicId)
    {
        return new LessonSectionProgressResponse(
            publicId,
            entity.IsCompleted,
            entity.TimeSpentSeconds,
            entity.StartedAt,
            entity.CompletedAt);
    }

    public static LessonBookmarkResponse ToBookmark(
        UserLessonBookmark entity)
    {
        return new LessonBookmarkResponse(
            entity.Lesson.PublicId,
            entity.Lesson.Slug,
            entity.Lesson.TitleVi,
            entity.Lesson.CoverImageUrl,
            entity.CreatedAt);
    }
}
