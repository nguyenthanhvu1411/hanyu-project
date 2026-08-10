namespace HanYu.Domain.Entities.Lesson;

public class LessonPrerequisite
{
    public long LessonId { get; private set; }

    public long RequiredLessonId { get; private set; }

    public Lesson Lesson { get; private set; } = null!;

    public Lesson RequiredLesson { get; private set; } = null!;

    protected LessonPrerequisite()
    {
    }

    public LessonPrerequisite(
        long lessonId,
        long requiredLessonId)
    {
        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        if (requiredLessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(requiredLessonId));

        if (lessonId == requiredLessonId)
            throw new ArgumentException(
                "Lesson không thể prerequisite chính nó.");

        LessonId = lessonId;
        RequiredLessonId = requiredLessonId;
    }
}
