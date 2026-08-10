namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record UpdateVocabularyRequest(
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
    long? AudioAssetId,
    int Version);
