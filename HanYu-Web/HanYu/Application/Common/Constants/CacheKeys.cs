namespace HanYu.Application.Common.Constants;

public static class CacheKeys
{
    public static string User(Guid userId) =>
        $"user:{userId}";

    public static string UserProfile(Guid userId) =>
        $"user:{userId}:profile";

    public static string Lesson(Guid lessonId) =>
        $"lesson:{lessonId}";

    public static string Vocabulary(Guid vocabularyId) =>
        $"vocabulary:{vocabularyId}";

    public static string Quiz(Guid quizId) =>
        $"quiz:{quizId}";
}