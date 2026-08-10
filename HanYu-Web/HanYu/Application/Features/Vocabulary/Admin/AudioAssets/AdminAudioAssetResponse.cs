using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.AudioAssets;

public sealed record AdminAudioAssetResponse(
    long Id,
    string StoragePath,
    string? PublicUrl,
    AudioAssetKind Kind,
    string MimeType,
    long? FileSizeBytes,
    int? DurationMs,
    string? Voice,
    string? Provider,
    string? LanguageCode,
    string? Checksum,
    ContentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
