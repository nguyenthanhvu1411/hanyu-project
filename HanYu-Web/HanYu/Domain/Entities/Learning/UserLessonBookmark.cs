using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Lesson;

namespace HanYu.Domain.Entities.Learning;

public class UserLessonBookmark
{
    public Guid UserId { get; private set; }

    public long LessonId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public User User { get; private set; } = null!;

    public HanYu.Domain.Entities.Lesson.Lesson Lesson { get; private set; } = null!;

    protected UserLessonBookmark()
    {
    }

    public UserLessonBookmark(
        Guid userId,
        long lessonId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        UserId = userId;
        LessonId = lessonId;
    }
}
