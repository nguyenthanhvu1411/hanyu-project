namespace HanYu.Application.Features.Vocabulary.Admin.Examples;

public sealed record CreateVocabularyExampleRequest(
    string SentenceZh,
    string SentencePinyin,
    string SentenceVi,
    short Difficulty,
    long? AudioAssetId,
    string? SourceNote);
