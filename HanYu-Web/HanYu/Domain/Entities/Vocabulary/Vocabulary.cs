using HanYu.Domain.Constants;
using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

public class Vocabulary : AuditableEntity
{
    public long HskLevelId { get; private set; }

    public long? PartOfSpeechId { get; private set; }

    public long? TopicId { get; private set; }

    public long? AudioAssetId { get; private set; }

    public string Simplified { get; private set; }
        = string.Empty;

    public string? Traditional { get; private set; }

    public string Pinyin { get; private set; }
        = string.Empty;

    public string PinyinNormalized { get; private set; }
        = string.Empty;

    public string PrimaryMeaningVi { get; private set; }
        = string.Empty;

    public string? NotesVi { get; private set; }

    public short Difficulty { get; private set; }
        = 1;

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public int Version { get; private set; }
        = 1;

    public DateTimeOffset? PublishedAt { get; private set; }

    public HskLevel HskLevel { get; private set; }
        = null!;

    public PartOfSpeech? PartOfSpeech { get; private set; }

    public Topic? Topic { get; private set; }

    public AudioAsset? AudioAsset { get; private set; }

    public ICollection<VocabularyMeaning> Meanings
    {
        get;
        private set;
    } = new List<VocabularyMeaning>();

    public ICollection<VocabularyExample> Examples
    {
        get;
        private set;
    } = new List<VocabularyExample>();

    protected Vocabulary()
    {
    }

    public Vocabulary(
        long hskLevelId,
        string simplified,
        string pinyin,
        string pinyinNormalized,
        string primaryMeaningVi,
        short difficulty = 1,
        string? traditional = null,
        long? partOfSpeechId = null,
        long? topicId = null,
        long? audioAssetId = null,
        string? notesVi = null)
    {
        Version = 0;

        Update(
            hskLevelId,
            simplified,
            traditional,
            pinyin,
            pinyinNormalized,
            primaryMeaningVi,
            notesVi,
            difficulty,
            partOfSpeechId,
            topicId,
            audioAssetId);
    }

    public void Update(
        long hskLevelId,
        string simplified,
        string? traditional,
        string pinyin,
        string pinyinNormalized,
        string primaryMeaningVi,
        string? notesVi,
        short difficulty,
        long? partOfSpeechId,
        long? topicId,
        long? audioAssetId)
    {
        SetHskLevel(
            hskLevelId);

        Simplified =
            NormalizeRequired(
                simplified,
                VocabularyConstants.MaxSimplifiedLength,
                nameof(simplified));

        Traditional =
            NormalizeOptional(
                traditional,
                VocabularyConstants.MaxTraditionalLength,
                nameof(traditional));

        Pinyin =
            NormalizeRequired(
                pinyin,
                VocabularyConstants.MaxPinyinLength,
                nameof(pinyin));

        PinyinNormalized =
            NormalizeRequired(
                pinyinNormalized,
                VocabularyConstants.MaxPinyinNormalizedLength,
                nameof(pinyinNormalized));

        PrimaryMeaningVi =
            NormalizeRequired(
                primaryMeaningVi,
                VocabularyConstants.MaxPrimaryMeaningLength,
                nameof(primaryMeaningVi));

        NotesVi =
            NormalizeOptional(
                notesVi,
                VocabularyConstants.MaxNotesLength,
                nameof(notesVi));

        ValidateDifficulty(
            difficulty);

        ValidateOptionalId(
            partOfSpeechId,
            nameof(partOfSpeechId));

        ValidateOptionalId(
            topicId,
            nameof(topicId));

        ValidateOptionalId(
            audioAssetId,
            nameof(audioAssetId));

        Difficulty =
            difficulty;

        PartOfSpeechId =
            partOfSpeechId;

        TopicId =
            topicId;

        AudioAssetId =
            audioAssetId;

        IncrementVersion();
        MarkUpdated();
    }

    public void ChangeAudio(
        long? audioAssetId)
    {
        ValidateOptionalId(
            audioAssetId,
            nameof(audioAssetId));

        if (AudioAssetId ==
            audioAssetId)
        {
            return;
        }

        AudioAssetId =
            audioAssetId;

        IncrementVersion();
        MarkUpdated();
    }

    public void SubmitForReview()
    {
        if (Status !=
            ContentStatus.Draft)
        {
            throw new InvalidOperationException(
                "Chỉ vocabulary ở trạng thái Draft mới có thể gửi duyệt.");
        }

        Status =
            ContentStatus.Review;

        IncrementVersion();
        MarkUpdated();
    }

    public void Approve()
    {
        if (Status !=
            ContentStatus.Review)
        {
            throw new InvalidOperationException(
                "Chỉ vocabulary đang Review mới có thể approve.");
        }

        Status =
            ContentStatus.Approved;

        IncrementVersion();
        MarkUpdated();
    }

    public void Publish()
    {
        if (Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Vocabulary phải được Approved trước khi publish.");
        }

        Status =
            ContentStatus.Published;

        PublishedAt =
            DateTimeOffset.UtcNow;

        IncrementVersion();
        MarkUpdated();
    }

    public void Archive()
    {
        if (Status ==
            ContentStatus.Archived)
        {
            return;
        }

        if (Status !=
                ContentStatus.Published &&
            Status !=
                ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Chỉ vocabulary Published hoặc Approved mới có thể archive.");
        }

        Status =
            ContentStatus.Archived;

        IncrementVersion();
        MarkUpdated();
    }

    public void Restore()
    {
        if (Status !=
            ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chỉ vocabulary Archived mới có thể restore.");
        }

        Status =
            ContentStatus.Draft;

        PublishedAt =
            null;

        IncrementVersion();
        MarkUpdated();
    }

    public void ReturnToDraft()
    {
        if (Status ==
            ContentStatus.Draft)
        {
            return;
        }

        Status =
            ContentStatus.Draft;

        PublishedAt =
            null;

        IncrementVersion();
        MarkUpdated();
    }

    private void IncrementVersion()
    {
        checked
        {
            Version++;
        }
    }

    private void SetHskLevel(long hskLevelId)
    {
        if (hskLevelId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hskLevelId));
        }

        HskLevelId =
            hskLevelId;
    }

    private static void ValidateDifficulty(
        short difficulty)
    {
        if (difficulty <
                VocabularyConstants.MinDifficulty ||
            difficulty >
                VocabularyConstants.MaxDifficulty)
        {
            throw new ArgumentOutOfRangeException(
                nameof(difficulty),
                $"Difficulty phải từ " +
                $"{VocabularyConstants.MinDifficulty} đến " +
                $"{VocabularyConstants.MaxDifficulty}.");
        }
    }

    private static void ValidateOptionalId(
        long? value,
        string parameterName)
    {
        if (value.HasValue &&
            value.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName);
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

        if (value.Length >
            maxLength)
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

        if (value.Length >
            maxLength)
        {
            throw new ArgumentException(
                $"{parameterName} vượt quá {maxLength} ký tự.",
                parameterName);
        }

        return value;
    }
}
