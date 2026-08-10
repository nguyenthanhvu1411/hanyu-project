using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed record AdminLessonResponse(
    long Id,
    Guid PublicId,
    long HskLevelId,
    string HskCode,
    string HskNameVi,
    long? TopicId,
    string? TopicSlug,
    string? TopicNameVi,
    long? CourseId,
    Guid? CoursePublicId,
    string? CourseTitleVi,
    long? CourseChapterId,
    Guid? CourseChapterPublicId,
    string? CourseChapterTitleVi,
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
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
