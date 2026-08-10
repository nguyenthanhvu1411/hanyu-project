namespace HanYu.Application.Features.Course.Admin.Chapters.Lessons;

public sealed class AssignLessonToChapterRequest
{
    public long LessonId { get; init; }

    public int SortOrder { get; init; }
}
