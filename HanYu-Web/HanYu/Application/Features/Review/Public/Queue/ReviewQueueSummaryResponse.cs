namespace HanYu.Application.Features.Review.Public.Queue;

public sealed record ReviewQueueSummaryResponse(
    int DueCount,
    int NewCount,
    int LearningCount,
    int KnownCount,
    int MasteredCount);
