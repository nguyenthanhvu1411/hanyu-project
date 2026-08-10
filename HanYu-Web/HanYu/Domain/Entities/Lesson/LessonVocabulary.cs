using HanYu.Domain.Entities.Vocabulary;

namespace HanYu.Domain.Entities.Lesson;

public class LessonVocabulary
{
    public long LessonId { get; private set; }

    public long VocabularyId { get; private set; }

    public short SortOrder { get; private set; }

    public bool IsRequired { get; private set; }
        = true;

    public Lesson Lesson { get; private set; } = null!;

    public HanYu.Domain.Entities.Vocabulary.Vocabulary Vocabulary { get; private set; } = null!;

    protected LessonVocabulary()
    {
    }

    public LessonVocabulary(
        long lessonId,
        long vocabularyId,
        short sortOrder,
        bool isRequired = true)
    {
        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        LessonId = lessonId;
        VocabularyId = vocabularyId;
        SortOrder = sortOrder;
        IsRequired = isRequired;
    }

    public void ChangeOrder(short sortOrder)
    {
        if (sortOrder < 0)
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));

        SortOrder = sortOrder;
    }

    public void SetRequired(bool required)
    {
        IsRequired = required;
    }
}
