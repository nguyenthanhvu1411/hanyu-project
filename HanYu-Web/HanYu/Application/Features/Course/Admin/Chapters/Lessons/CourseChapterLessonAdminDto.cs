using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Course.Admin.Chapters.Lessons;

public sealed record CourseChapterLessonAdminDto(
    long Id,
    Guid PublicId,

    long? CourseChapterId,

    string Slug,
    string TitleVi,

    int SortOrder,

    short EstimatedMinutes,
    short Difficulty,

    ContentStatus Status,

    int Version,

    DateTimeOffset? PublishedAt,

    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
