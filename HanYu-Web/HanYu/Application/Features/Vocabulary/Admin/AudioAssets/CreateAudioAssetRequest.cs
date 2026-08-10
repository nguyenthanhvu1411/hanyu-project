using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.AudioAssets;

public sealed record CreateAudioAssetRequest(
    string StoragePath,
    AudioAssetKind Kind,
    string MimeType);
