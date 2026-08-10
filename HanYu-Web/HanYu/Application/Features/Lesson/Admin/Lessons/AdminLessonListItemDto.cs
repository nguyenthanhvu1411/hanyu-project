using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed record AdminLessonListItemDto(
    long Id,
    Guid PublicId,

    long? CourseId,
    string? CourseTitleVi,

    long? ChapterId,
    string? ChapterTitleVi,

    long HskLevelId,
    string? HskCode,

    long? TopicId,

    string Slug,
    string TitleVi,
    string? ShortDescriptionVi,

    int SortOrder,
    short EstimatedMinutes,
    short Difficulty,

    bool IsFeatured,

    ContentStatus Status,
    int Version,

    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
