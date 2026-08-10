using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.AI;

public class AiConversation : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public long? LessonId { get; private set; }

    public long? VocabularyId { get; private set; }

    public string? Title { get; private set; }

    public AiConversationStatus Status { get; private set; }
        = AiConversationStatus.Active;

    public int MessageCount { get; private set; }

    public DateTimeOffset LastMessageAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public ICollection<AiConversationMessage> Messages { get; private set; }
        = new List<AiConversationMessage>();

    protected AiConversation()
    {
    }

    public AiConversation(
        Guid userId,
        string? title = null,
        long? lessonId = null,
        long? vocabularyId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId không hợp lệ.", nameof(userId));

        UserId = userId;
        Title = NormalizeNullable(title);
        LessonId = lessonId;
        VocabularyId = vocabularyId;
    }

    public void UpdateTitle(string? title)
    {
        Title = NormalizeNullable(title);
        MarkUpdated();
    }

    public void ChangeContext(long? lessonId, long? vocabularyId)
    {
        LessonId = lessonId;
        VocabularyId = vocabularyId;
        MarkUpdated();
    }

    public void RegisterMessage()
    {
        MessageCount++;
        LastMessageAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Archive()
    {
        if (Status == AiConversationStatus.Archived)
            return;

        Status = AiConversationStatus.Archived;
        MarkUpdated();
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
