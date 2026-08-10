using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Public.Conversations;

public sealed record AiConversationResponse(
    Guid PublicId,
    string? Title,
    AiConversationStatus Status,
    int MessageCount,
    DateTimeOffset LastMessageAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record AiConversationDetailResponse(
    Guid PublicId,
    string? Title,
    AiConversationStatus Status,
    int MessageCount,
    DateTimeOffset LastMessageAt,
    IReadOnlyCollection<AiConversationMessageResponse> Messages);

public sealed record AiConversationMessageResponse(
    Guid PublicId,
    AiMessageRole Role,
    string Content,
    DateTimeOffset CreatedAt);

public sealed record CreateAiConversationRequest(
    string? Title,
    Guid? LessonPublicId,
    Guid? VocabularyPublicId);

public sealed record SendAiMessageRequest(
    string Content);
