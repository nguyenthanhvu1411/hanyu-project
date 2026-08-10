namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyDetailResponse(
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PinyinNormalized,
    string PrimaryMeaningVi,
    string? NotesVi,
    long HskLevel,
    string HskCode,
    string HskNameVi,
    short Difficulty,
    string? PartOfSpeech,
    string? PartOfSpeechNameVi,
    string? Topic,
    string? TopicNameVi,
    string? AudioUrl,
    IReadOnlyCollection<VocabularyMeaningResponse> Meanings,
    IReadOnlyCollection<VocabularyExampleResponse> Examples,
    IReadOnlyCollection<VocabularyRelationResponse> Relations);
