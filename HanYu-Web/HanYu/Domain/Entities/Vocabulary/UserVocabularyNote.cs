using HanYu.Domain.Constants;
using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Domain.Entities.Vocabulary;

public class UserVocabularyNote : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public long VocabularyId { get; private set; }

    public string Content { get; private set; }
        = string.Empty;

    public bool IsPinned { get; private set; }

    public User User { get; private set; }
        = null!;

    public Vocabulary Vocabulary { get; private set; }
        = null!;

    protected UserVocabularyNote()
    {
    }

    public UserVocabularyNote(
        Guid userId,
        long vocabularyId,
        string content)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));
        }

        if (vocabularyId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));
        }

        UserId =
            userId;

        VocabularyId =
            vocabularyId;

        UpdateContent(
            content);
    }

    public void UpdateContent(
        string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException(
                "Nội dung ghi chú không được để trống.",
                nameof(content));
        }

        content =
            content.Trim();

        if (content.Length >
            VocabularyConstants.MaxUserNoteLength)
        {
            throw new ArgumentException(
                $"Ghi chú không được vượt quá " +
                $"{VocabularyConstants.MaxUserNoteLength} ký tự.",
                nameof(content));
        }

        Content =
            content;

        MarkUpdated();
    }

    public void SetPinned(
        bool pinned)
    {
        if (IsPinned == pinned)
        {
            return;
        }

        IsPinned =
            pinned;

        MarkUpdated();
    }

    public void Pin()
    {
        SetPinned(true);
    }

    public void Unpin()
    {
        SetPinned(false);
    }
}
