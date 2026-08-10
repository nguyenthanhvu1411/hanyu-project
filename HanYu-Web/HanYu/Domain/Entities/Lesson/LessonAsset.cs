using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Lesson;

public class LessonAsset : AuditableEntity
{
    public long LessonId { get; private set; }

    public long? AudioAssetId { get; private set; }

    public LessonAssetType AssetType { get; private set; }

    public string? Url { get; private set; }

    public string? CaptionVi { get; private set; }

    public int SortOrder { get; private set; }

    public Lesson Lesson { get; private set; } = null!;

    public AudioAsset? AudioAsset { get; private set; }

    protected LessonAsset()
    {
    }

    public LessonAsset(
        long lessonId,
        LessonAssetType assetType,
        int sortOrder = 0)
    {
        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        LessonId = lessonId;
        AssetType = assetType;
        SortOrder = sortOrder;
    }

    public void Update(
        string? url,
        string? captionVi,
        long? audioAssetId,
        int sortOrder)
    {
        if (audioAssetId.HasValue &&
            audioAssetId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(audioAssetId));
        }

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        url = Normalize(url);

        if (url?.Length > 2048)
            throw new ArgumentException(
                "URL quá dài.",
                nameof(url));

        Url = url;
        CaptionVi = Normalize(captionVi);
        AudioAssetId = audioAssetId;
        SortOrder = sortOrder;

        ValidateAssetSource();

        MarkUpdated();
    }

    public void AssignAudio(
        long audioAssetId)
    {
        if (audioAssetId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(audioAssetId));
        }

        AudioAssetId = audioAssetId;

        ValidateAssetSource();

        MarkUpdated();
    }

    public void RemoveAudio()
    {
        if (!AudioAssetId.HasValue)
            return;

        AudioAssetId = null;

        MarkUpdated();
    }

    public void ChangeOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder = sortOrder;

        MarkUpdated();
    }

    private void ValidateAssetSource()
    {
        if (AssetType == LessonAssetType.Audio &&
            !AudioAssetId.HasValue &&
            string.IsNullOrWhiteSpace(Url))
        {
            throw new InvalidOperationException(
                "Audio lesson asset phải có AudioAssetId hoặc Url.");
        }
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
