namespace HanYu.Application.Features.Lesson.Admin.Vocabulary;

public sealed record AttachLessonVocabularyRequest(
    long VocabularyId,
    short SortOrder,
    bool IsRequired);
