namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record ChangeVocabularyAudioRequest(
    long? AudioAssetId,
    int Version);
