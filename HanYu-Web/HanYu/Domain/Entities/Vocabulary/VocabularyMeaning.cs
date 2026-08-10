using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Vocabulary;

public class VocabularyMeaning : AuditableEntity
{
    public long VocabularyId { get; private set; }

    public string MeaningVi { get; private set; }
        = string.Empty;

    public short SenseOrder { get; private set; }

    public string? UsageNoteVi { get; private set; }

    public Vocabulary Vocabulary { get; private set; } = null!;

    protected VocabularyMeaning()
    {
    }

    public VocabularyMeaning(
        long vocabularyId,
        string meaningVi,
        short senseOrder,
        string? usageNoteVi = null)
    {
        if (vocabularyId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(vocabularyId));

        VocabularyId = vocabularyId;

        Update(
            meaningVi,
            senseOrder,
            usageNoteVi);
    }

    public void Update(
        string meaningVi,
        short senseOrder,
        string? usageNoteVi)
    {
        if (string.IsNullOrWhiteSpace(meaningVi))
            throw new ArgumentException(
                "MeaningVi không được để trống.",
                nameof(meaningVi));

        if (senseOrder <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(senseOrder),
                "SenseOrder phải lớn hơn 0.");

        meaningVi = meaningVi.Trim();

        if (meaningVi.Length > 500)
            throw new ArgumentException(
                "MeaningVi quá dài.",
                nameof(meaningVi));

        MeaningVi = meaningVi;
        SenseOrder = senseOrder;
        UsageNoteVi = Normalize(usageNoteVi);

        MarkUpdated();
    }

    public void ChangeOrder(
        short senseOrder)
    {
        if (senseOrder <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(senseOrder));

        SenseOrder = senseOrder;

        MarkUpdated();
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
