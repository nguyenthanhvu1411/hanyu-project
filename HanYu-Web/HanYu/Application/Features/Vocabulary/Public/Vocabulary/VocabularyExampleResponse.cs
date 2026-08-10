namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyExampleResponse(
    string SentenceZh,
    string SentencePinyin,
    string SentenceVi,
    short Difficulty,
    string? AudioUrl);
