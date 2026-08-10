using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Vocabulary;

public class VocabularyRelation : AuditableEntity
{
    public long VocabularyId { get; private set; }

    public long RelatedVocabularyId { get; private set; }

    public VocabularyRelationType RelationType { get; private set; }

    public string? NoteVi { get; private set; }

    public Vocabulary Vocabulary { get; private set; } = null!;

    public Vocabulary RelatedVocabulary { get; private set; } = null!;

    protected VocabularyRelation()
    {
    }

    public VocabularyRelation(
        long vocabularyId,
        long relatedVocabularyId,
        VocabularyRelationType relationType,
        string? noteVi = null)
    {
        ValidateIds(
            vocabularyId,
            relatedVocabularyId);

        VocabularyId = vocabularyId;
        RelatedVocabularyId = relatedVocabularyId;
        RelationType = relationType;
        NoteVi = Normalize(noteVi);
    }

    public void UpdateRelation(
        VocabularyRelationType relationType,
        string? noteVi)
    {
        RelationType = relationType;
        NoteVi = Normalize(noteVi);

        MarkUpdated();
    }

    public void ChangeRelatedVocabulary(
        long relatedVocabularyId)
    {
        ValidateIds(
            VocabularyId,
            relatedVocabularyId);

        RelatedVocabularyId =
            relatedVocabularyId;

        MarkUpdated();
    }

    private static void ValidateIds(
        long vocabularyId,
        long relatedVocabularyId)
    {
        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        if (relatedVocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(relatedVocabularyId));

        if (vocabularyId == relatedVocabularyId)
            throw new ArgumentException(
                "Vocabulary không thể relation với chính nó.");
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
