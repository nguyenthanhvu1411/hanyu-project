using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.AI;

public class AiRequest : BaseEntity
{
    public Guid? UserId { get; private set; }

    public long? ConversationId { get; private set; }

    public long? VocabularyId { get; private set; }

    public long? LessonId { get; private set; }

    public long? QuizAttemptAnswerId { get; private set; }

    public AiFeatureType FeatureType { get; private set; }

    public string Provider { get; private set; } = string.Empty;

    public string Model { get; private set; } = string.Empty;

    public string? RequestHash { get; private set; }

    public string? PromptVersion { get; private set; }

    public int InputTokens { get; private set; }

    public int OutputTokens { get; private set; }

    public int TotalTokens { get; private set; }

    public decimal? EstimatedCostUsd { get; private set; }

    public int? LatencyMs { get; private set; }

    public AiRequestStatus Status { get; private set; }
        = AiRequestStatus.Pending;

    public string? ErrorCode { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; private set; }

    protected AiRequest()
    {
    }

    public AiRequest(
        Guid? userId,
        AiFeatureType featureType,
        string provider,
        string model,
        string? requestHash = null,
        string? promptVersion = null,
        long? conversationId = null,
        long? vocabularyId = null,
        long? lessonId = null,
        long? quizAttemptAnswerId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException(
                "Provider không được để trống.",
                nameof(provider));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException(
                "Model không được để trống.",
                nameof(model));

        UserId = userId;
        FeatureType = featureType;
        Provider = provider.Trim();
        Model = model.Trim();
        RequestHash = NormalizeNullable(requestHash);
        PromptVersion = NormalizeNullable(promptVersion);

        ConversationId = conversationId;
        VocabularyId = vocabularyId;
        LessonId = lessonId;
        QuizAttemptAnswerId = quizAttemptAnswerId;
    }

    public void Complete(
        int inputTokens,
        int outputTokens,
        decimal? estimatedCostUsd,
        int latencyMs)
    {
        if (Status != AiRequestStatus.Pending)
            throw new InvalidOperationException(
                "AI request đã kết thúc và không thể complete lại.");

        if (inputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(inputTokens));

        if (outputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(outputTokens));

        if (latencyMs < 0)
            throw new ArgumentOutOfRangeException(nameof(latencyMs));

        if (estimatedCostUsd < 0)
            throw new ArgumentOutOfRangeException(nameof(estimatedCostUsd));

        InputTokens = inputTokens;
        OutputTokens = outputTokens;
        TotalTokens = inputTokens + outputTokens;

        EstimatedCostUsd = estimatedCostUsd;
        LatencyMs = latencyMs;

        Status = AiRequestStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Fail(
        string? errorCode,
        string? errorMessage,
        int? latencyMs = null)
    {
        if (Status != AiRequestStatus.Pending)
            return;

        if (latencyMs < 0)
            throw new ArgumentOutOfRangeException(nameof(latencyMs));

        Status = AiRequestStatus.Failed;
        ErrorCode = NormalizeNullable(errorCode);
        ErrorMessage = NormalizeNullable(errorMessage);
        LatencyMs = latencyMs;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status != AiRequestStatus.Pending)
            return;

        Status = AiRequestStatus.Cancelled;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
