using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed class AdminLessonQuery
{
    public string? Search { get; init; }

    public long? CourseId { get; init; }

    public long? ChapterId { get; init; }

    public long? HskLevelId { get; init; }

    public long? TopicId { get; init; }

    public ContentStatus? Status { get; init; }

    public bool? IsFeatured { get; init; }

    public bool IncludeDeleted { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
