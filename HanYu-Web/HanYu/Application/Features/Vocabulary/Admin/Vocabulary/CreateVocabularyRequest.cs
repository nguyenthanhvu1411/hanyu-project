namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record CreateVocabularyRequest(
    long HskLevelId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PinyinNormalized,
    string PrimaryMeaningVi,
    string? NotesVi,
    short Difficulty,
    long? PartOfSpeechId,
    long? TopicId,
    long? AudioAssetId);
