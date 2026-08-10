namespace HanYu.Application.Features.Course.Admin.Chapters.Lessons;

public sealed class MoveLessonToChapterRequest
{
    public long TargetChapterId { get; init; }

    public int SortOrder { get; init; }
}
