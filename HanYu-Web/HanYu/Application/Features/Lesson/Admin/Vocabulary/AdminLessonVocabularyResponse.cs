namespace HanYu.Application.Features.Lesson.Admin.Vocabulary;

public sealed record AdminLessonVocabularyResponse(
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    short SortOrder,
    bool IsRequired);
