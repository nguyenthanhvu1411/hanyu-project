namespace HanYu.Application.Features.Course.Admin.Chapters.Lessons;

public sealed class ReorderChapterLessonsRequest
{
    public IReadOnlyList<ReorderChapterLessonItemRequest> Items { get; init; }
        = [];
}

public sealed class ReorderChapterLessonItemRequest
{
    public long LessonId { get; init; }

    public int SortOrder { get; init; }
}
