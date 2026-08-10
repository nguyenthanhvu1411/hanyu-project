namespace HanYu.Application.Interfaces.Gamification;

public interface IAchievementEvaluator
{
    Task EvaluateAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
