using HanYu.Domain.Constants;
using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

public class VocabularyExample : AuditableEntity
{
    public long VocabularyId { get; private set; }

    public long? AudioAssetId { get; private set; }

    public string SentenceZh { get; private set; }
        = string.Empty;

    public string SentencePinyin { get; private set; }
        = string.Empty;

    public string SentenceVi { get; private set; }
        = string.Empty;

    public short Difficulty { get; private set; }
        = 1;

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public string? SourceNote { get; private set; }

    public Vocabulary Vocabulary { get; private set; }
        = null!;

    public AudioAsset? AudioAsset { get; private set; }

    protected VocabularyExample()
    {
    }

    public VocabularyExample(
        long vocabularyId,
        string sentenceZh,
        string sentencePinyin,
        string sentenceVi,
        short difficulty = 1,
        long? audioAssetId = null,
        string? sourceNote = null)
    {
        if (vocabularyId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));
        }

        VocabularyId =
            vocabularyId;

        Update(
            sentenceZh,
            sentencePinyin,
            sentenceVi,
            difficulty,
            audioAssetId,
            sourceNote);
    }

    public void Update(
        string sentenceZh,
        string sentencePinyin,
        string sentenceVi,
        short difficulty,
        long? audioAssetId,
        string? sourceNote)
    {
        SentenceZh =
            NormalizeRequired(
                sentenceZh,
                VocabularyConstants.MaxSentenceLength,
                nameof(sentenceZh));

        SentencePinyin =
            NormalizeRequired(
                sentencePinyin,
                VocabularyConstants.MaxSentenceLength,
                nameof(sentencePinyin));

        SentenceVi =
            NormalizeRequired(
                sentenceVi,
                VocabularyConstants.MaxExampleTranslationLength,
                nameof(sentenceVi));

        if (difficulty <
                VocabularyConstants.MinDifficulty ||
            difficulty >
                VocabularyConstants.MaxDifficulty)
        {
            throw new ArgumentOutOfRangeException(
                nameof(difficulty));
        }

        ValidateAudioAssetId(audioAssetId);

        Difficulty =
            difficulty;

        AudioAssetId =
            audioAssetId;

        SourceNote =
            NormalizeOptional(
                sourceNote,
                VocabularyConstants.MaxSourceNoteLength,
                nameof(sourceNote));

        MarkUpdated();
    }

    public void ChangeAudio(long? audioAssetId)
    {
        ValidateAudioAssetId(audioAssetId);

        if (AudioAssetId == audioAssetId)
        {
            return;
        }

        AudioAssetId = audioAssetId;
        MarkUpdated();
    }

    public void SubmitForReview()
    {
        if (Status !=
            ContentStatus.Draft)
        {
            throw new InvalidOperationException(
                "Example phải ở Draft trước khi gửi review.");
        }

        Status =
            ContentStatus.Review;

        MarkUpdated();
    }

    public void Approve()
    {
        if (Status !=
            ContentStatus.Review)
        {
            throw new InvalidOperationException(
                "Example phải ở Review trước khi approve.");
        }

        Status =
            ContentStatus.Approved;

        MarkUpdated();
    }

    public void Publish()
    {
        if (Status !=
            ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Example phải được Approved trước khi publish.");
        }

        Status =
            ContentStatus.Published;

        MarkUpdated();
    }

    public void Archive()
    {
        if (Status ==
            ContentStatus.Archived)
        {
            return;
        }

        Status =
            ContentStatus.Archived;

        MarkUpdated();
    }

    public void Restore()
    {
        if (Status !=
            ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chỉ example Archived mới có thể restore.");
        }

        Status =
            ContentStatus.Draft;

        MarkUpdated();
    }

    private static void ValidateAudioAssetId(long? audioAssetId)
    {
        if (audioAssetId.HasValue &&
            audioAssetId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(audioAssetId));
        }
    }

    private static string NormalizeRequired(
        string value,
        int maxLength,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{parameterName} không được để trống.",
                parameterName);
        }

        value =
            value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException(
                $"{parameterName} vượt quá {maxLength} ký tự.",
                parameterName);
        }

        return value;
    }

    private static string? NormalizeOptional(
        string? value,
        int maxLength,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value =
            value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException(
                $"{parameterName} vượt quá {maxLength} ký tự.",
                parameterName);
        }

        return value;
    }
}
