namespace HanYu.Application.Interfaces.Analytics;

public interface ILearningAnalyticsCollector
{
    Task RegisterLessonStartedAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RegisterLessonCompletedAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RegisterReviewAsync(
        Guid userId,
        bool wasCorrect,
        CancellationToken cancellationToken = default);

    Task RegisterVocabularyLearnedAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RegisterQuizAttemptAsync(
        Guid userId,
        bool passed,
        CancellationToken cancellationToken = default);

    Task RegisterAiInteractionAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RegisterXpAsync(
        Guid userId,
        int xp,
        CancellationToken cancellationToken = default);

    Task AddLearningTimeAsync(
        Guid userId,
        int seconds,
        CancellationToken cancellationToken = default);
}
