using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

public class AudioAsset : AuditableEntity
{
    public string StoragePath { get; private set; }
        = string.Empty;

    public string? PublicUrl { get; private set; }

    public AudioAssetKind Kind { get; private set; }

    public string MimeType { get; private set; }
        = string.Empty;

    public long? FileSizeBytes { get; private set; }

    public int? DurationMs { get; private set; }

    public string? Voice { get; private set; }

    public string? Provider { get; private set; }

    public string? LanguageCode { get; private set; }

    public string? Checksum { get; private set; }

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    protected AudioAsset()
    {
    }

    public AudioAsset(
        string storagePath,
        AudioAssetKind kind,
        string mimeType)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException(
                "StoragePath không được để trống.",
                nameof(storagePath));

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException(
                "MimeType không được để trống.",
                nameof(mimeType));

        StoragePath = storagePath.Trim();
        Kind = kind;
        MimeType = mimeType.Trim().ToLowerInvariant();
    }

    public void UpdateFileInfo(
        string storagePath,
        string mimeType,
        long? fileSizeBytes,
        int? durationMs,
        string? checksum = null)
    {
        EnsureEditable();

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException(
                "StoragePath không được để trống.",
                nameof(storagePath));

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException(
                "MimeType không được để trống.",
                nameof(mimeType));

        if (fileSizeBytes.HasValue &&
            fileSizeBytes.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileSizeBytes));
        }

        if (durationMs.HasValue &&
            durationMs.Value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationMs));
        }

        StoragePath = storagePath.Trim();
        MimeType = mimeType.Trim().ToLowerInvariant();
        FileSizeBytes = fileSizeBytes;
        DurationMs = durationMs;
        Checksum = Normalize(checksum);

        MarkUpdated();
    }

    public void UpdateAudioMetadata(
        string? voice,
        string? provider,
        string? languageCode)
    {
        EnsureEditable();

        Voice = Normalize(voice);
        Provider = Normalize(provider);
        LanguageCode = Normalize(languageCode);

        MarkUpdated();
    }

    public void UpdatePublicUrl(
        string? publicUrl)
    {
        EnsureEditable();

        publicUrl = Normalize(publicUrl);

        if (publicUrl?.Length > 2048)
            throw new ArgumentException(
                "PublicUrl quá dài.",
                nameof(publicUrl));

        PublicUrl = publicUrl;

        MarkUpdated();
    }

    public void Publish()
    {
        if (Status == ContentStatus.Published)
            return;

        if (string.IsNullOrWhiteSpace(StoragePath) ||
            string.IsNullOrWhiteSpace(MimeType))
        {
            throw new InvalidOperationException(
                "AudioAsset chưa đủ dữ liệu để publish.");
        }

        Status = ContentStatus.Published;

        MarkUpdated();
    }

    public void Archive()
    {
        if (Status == ContentStatus.Archived)
            return;

        Status = ContentStatus.Archived;

        MarkUpdated();
    }

    private void EnsureEditable()
    {
        if (Status == ContentStatus.Archived)
            throw new InvalidOperationException(
                "AudioAsset đã Archived.");
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
