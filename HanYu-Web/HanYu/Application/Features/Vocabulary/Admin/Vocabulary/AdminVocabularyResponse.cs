using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record AdminVocabularyResponse(
    long Id,
    long HskLevelId,
    string HskCode,
    string HskNameVi,
    long? PartOfSpeechId,
    string? PartOfSpeechCode,
    string? PartOfSpeechNameVi,
    long? TopicId,
    string? TopicSlug,
    string? TopicNameVi,
    long? AudioAssetId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PinyinNormalized,
    string PrimaryMeaningVi,
    string? NotesVi,
    short Difficulty,
    ContentStatus Status,
    int Version,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
