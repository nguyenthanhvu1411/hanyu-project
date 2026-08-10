namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed class CreateLessonRequest
{
    public long? CourseChapterId { get; init; }

    public short HskLevelId { get; init; }

    public long? TopicId { get; init; }

    public string Slug { get; init; }
        = string.Empty;

    public string TitleVi { get; init; }
        = string.Empty;

    public string? ShortDescriptionVi { get; init; }

    public string? DescriptionVi { get; init; }

    public string? ObjectiveVi { get; init; }

    public string? CoverImageUrl { get; init; }

    public int SortOrder { get; init; }

    public short EstimatedMinutes { get; init; }
        = 15;

    public short Difficulty { get; init; }
        = 1;

    public bool IsFeatured { get; init; }
}
