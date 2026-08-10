using HanYu.Application.Interfaces.Gamification;

namespace HanYu.Infrastructure.Gamification;

public sealed class AchievementEvaluator : IAchievementEvaluator
{
    private readonly IGamificationService _gamificationService;

    public AchievementEvaluator(IGamificationService gamificationService)
    {
        _gamificationService = gamificationService;
    }

    public async Task EvaluateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // This is a placeholder for the actual achievement evaluation logic.
        // It would typically query user stats (completed lessons, perfect quizzes, current streak)
        // and call _gamificationService.UnlockAchievementAsync(...) for each met condition.
        await Task.CompletedTask;
    }
}
