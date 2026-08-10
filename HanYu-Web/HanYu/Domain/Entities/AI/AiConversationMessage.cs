using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.AI;

public class AiConversationMessage : BaseEntity
{
    public long ConversationId { get; private set; }

    public long? AiRequestId { get; private set; }

    public AiMessageRole Role { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public string? MetadataJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public AiConversation Conversation { get; private set; } = null!;

    protected AiConversationMessage()
    {
    }

    public AiConversationMessage(
        long conversationId,
        AiMessageRole role,
        string content,
        long? aiRequestId = null,
        string? metadataJson = null)
    {
        if (conversationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(conversationId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Nội dung message không được để trống.",
                nameof(content));

        ConversationId = conversationId;
        Role = role;
        Content = content.Trim();
        AiRequestId = aiRequestId;
        MetadataJson = string.IsNullOrWhiteSpace(metadataJson)
            ? null
            : metadataJson;
    }
}
