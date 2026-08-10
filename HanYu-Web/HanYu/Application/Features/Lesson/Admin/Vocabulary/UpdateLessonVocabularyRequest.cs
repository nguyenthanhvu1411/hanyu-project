namespace HanYu.Application.Features.Lesson.Admin.Vocabulary;

public sealed record UpdateLessonVocabularyRequest(
    short SortOrder,
    bool IsRequired);
