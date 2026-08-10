using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Features.Lesson.Admin.Sections;
using HanYu.Application.Features.Lesson.Admin.Assets;
using HanYu.Application.Features.Lesson.Admin.Vocabulary;
using HanYu.Application.Features.Lesson.Admin.Prerequisites;
using HanYu.Domain.Entities.Lesson;
using System;
using System.Linq;

namespace HanYu.Application.Features.Lesson.Mapping;

public static class LessonAdminMapper
{
    public static AdminLessonDetailDto ToDetailDto(
        HanYu.Domain.Entities.Lesson.Lesson lesson)
    {
        ArgumentNullException.ThrowIfNull(
            lesson);

        return new AdminLessonDetailDto(
            Id:
                lesson.Id,

            PublicId:
                lesson.PublicId,

            CourseId:
                lesson.CourseChapter?.CourseId,

            CoursePublicId:
                lesson.CourseChapter?.Course.PublicId,

            CourseTitleVi:
                lesson.CourseChapter?.Course.TitleVi,

            CourseChapterId:
                lesson.CourseChapterId,

            CourseChapterPublicId:
                lesson.CourseChapter?.PublicId,

            CourseChapterTitleVi:
                lesson.CourseChapter?.TitleVi,

            HskLevelId:
                lesson.HskLevelId,

            HskCode:
                lesson.HskLevel?.Code,

            HskNameVi:
                lesson.HskLevel?.NameVi,

            TopicId:
                lesson.TopicId,

            TopicNameVi:
                lesson.Topic?.NameVi,

            Slug:
                lesson.Slug,

            TitleVi:
                lesson.TitleVi,

            ShortDescriptionVi:
                lesson.ShortDescriptionVi,

            DescriptionVi:
                lesson.DescriptionVi,

            ObjectiveVi:
                lesson.ObjectiveVi,

            CoverImageUrl:
                lesson.CoverImageUrl,

            SortOrder:
                lesson.SortOrder,

            EstimatedMinutes:
                lesson.EstimatedMinutes,

            Difficulty:
                lesson.Difficulty,

            IsFeatured:
                lesson.IsFeatured,

            Status:
                lesson.Status,

            Version:
                lesson.Version,

            PublishedAt:
                lesson.PublishedAt,

            SectionCount:
                lesson.Sections.Count(
                    x => !x.IsDeleted),

            VocabularyCount:
                lesson.LessonVocabularies.Count,

            AssetCount:
                lesson.Assets.Count,

            PrerequisiteCount:
                lesson.Prerequisites.Count,

            CreatedAt:
                lesson.CreatedAt,

            CreatedById:
                lesson.CreatedById,

            UpdatedAt:
                lesson.UpdatedAt,

            UpdatedById:
                lesson.UpdatedById,

            DeletedAt:
                lesson.DeletedAt,

            DeletedById:
                lesson.DeletedById);
    }

    public static AdminLessonSectionResponse ToSectionResponse(LessonSection section)
    {
        return new AdminLessonSectionResponse(
            section.Id,
            section.PublicId,
            section.LessonId,
            section.SectionType,
            section.TitleVi,
            section.ContentVi,
            section.SortOrder,
            section.IsRequired,
            section.EstimatedSeconds,
            section.CreatedAt,
            section.UpdatedAt);
    }

    public static AdminLessonAssetResponse ToAssetResponse(LessonAsset asset)
    {
        return new AdminLessonAssetResponse(
            asset.Id,
            asset.PublicId,
            asset.LessonId,
            asset.AudioAssetId,
            asset.AssetType,
            asset.Url,
            asset.CaptionVi,
            asset.SortOrder,
            asset.CreatedAt,
            asset.UpdatedAt);
    }

    public static AdminLessonVocabularyResponse ToVocabularyResponse(LessonVocabulary vocab)
    {
        return new AdminLessonVocabularyResponse(
            vocab.VocabularyId,
            vocab.Vocabulary.PublicId,
            vocab.Vocabulary.Simplified,
            vocab.Vocabulary.Traditional,
            vocab.Vocabulary.Pinyin,
            vocab.Vocabulary.PrimaryMeaningVi,
            vocab.SortOrder,
            vocab.IsRequired);
    }

    public static AdminLessonPrerequisiteResponse ToPrerequisiteResponse(LessonPrerequisite prereq)
    {
        return new AdminLessonPrerequisiteResponse(
            prereq.RequiredLessonId,
            prereq.RequiredLesson.PublicId,
            prereq.RequiredLesson.Slug,
            prereq.RequiredLesson.TitleVi);
    }
}
