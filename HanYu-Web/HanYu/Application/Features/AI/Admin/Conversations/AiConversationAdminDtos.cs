using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Admin.Conversations;

public sealed record AdminAiConversationQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public AiConversationStatus? Status { get; init; }

    public string? Sort { get; init; }
        = "-updatedAt";
}

public sealed record AdminAiConversationResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    string? Title,
    AiConversationStatus Status,
    int MessageCount,
    DateTimeOffset LastMessageAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
