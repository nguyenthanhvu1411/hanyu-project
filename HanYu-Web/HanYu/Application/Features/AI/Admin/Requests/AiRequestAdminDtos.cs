using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Admin.Requests;

public sealed record AdminAiRequestQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public AiFeatureType? FeatureType { get; init; }

    public AiRequestStatus? Status { get; init; }

    public string? Provider { get; init; }

    public string? Model { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; }
        = "-requestedAt";
}

public sealed record AdminAiRequestResponse(
    long Id,
    Guid PublicId,
    Guid? UserId,
    long? ConversationId,
    long? VocabularyId,
    long? LessonId,
    long? QuizAttemptAnswerId,
    AiFeatureType FeatureType,
    string Provider,
    string Model,
    string? RequestHash,
    string? PromptVersion,
    int InputTokens,
    int OutputTokens,
    int TotalTokens,
    decimal? EstimatedCostUsd,
    int? LatencyMs,
    AiRequestStatus Status,
    string? ErrorCode,
    string? ErrorMessage,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt);
    
public sealed record AdminAiDashboardResponse(
    long RequestsToday,
    long CompletedToday,
    long FailedToday,
    long CancelledToday,
    long InputTokensToday,
    long OutputTokensToday,
    long TotalTokensToday,
    decimal EstimatedCostUsdToday,
    decimal AverageLatencyMs);
