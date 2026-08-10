using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed record AdminLessonDetailDto(
    long Id,
    Guid PublicId,

    long? CourseId,
    Guid? CoursePublicId,
    string? CourseTitleVi,

    long? CourseChapterId,
    Guid? CourseChapterPublicId,
    string? CourseChapterTitleVi,

    long HskLevelId,
    string? HskCode,
    string? HskNameVi,

    long? TopicId,
    string? TopicNameVi,

    string Slug,
    string TitleVi,

    string? ShortDescriptionVi,
    string? DescriptionVi,
    string? ObjectiveVi,

    string? CoverImageUrl,

    int SortOrder,
    short EstimatedMinutes,
    short Difficulty,

    bool IsFeatured,

    ContentStatus Status,
    int Version,

    DateTimeOffset? PublishedAt,

    int SectionCount,
    int VocabularyCount,
    int AssetCount,
    int PrerequisiteCount,

    DateTimeOffset CreatedAt,
    Guid? CreatedById,

    DateTimeOffset UpdatedAt,
    Guid? UpdatedById,

    DateTimeOffset? DeletedAt,
    Guid? DeletedById);
