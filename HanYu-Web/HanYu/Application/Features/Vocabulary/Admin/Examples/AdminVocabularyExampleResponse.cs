using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Examples;

public sealed record AdminVocabularyExampleResponse(
    long Id,
    long VocabularyId,
    long? AudioAssetId,
    string SentenceZh,
    string SentencePinyin,
    string SentenceVi,
    short Difficulty,
    ContentStatus Status,
    string? SourceNote,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
