namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonVocabularyResponse(
    Guid PublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    short SortOrder,
    bool IsRequired);
