namespace HanYu.Application.Features.Vocabulary.Admin.AudioAssets;

public sealed record UpdateAudioAssetRequest(
    string StoragePath,
    string MimeType,
    long? FileSizeBytes,
    int? DurationMs,
    string? Checksum,
    string? Voice,
    string? Provider,
    string? LanguageCode,
    string? PublicUrl);
