namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyListItemResponse(
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PinyinNormalized,
    string PrimaryMeaningVi,
    long HskLevel,
    string HskCode,
    short Difficulty,
    string? PartOfSpeech,
    string? Topic,
    string? AudioUrl);
