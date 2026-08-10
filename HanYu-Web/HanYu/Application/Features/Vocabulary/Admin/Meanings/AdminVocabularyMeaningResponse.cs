namespace HanYu.Application.Features.Vocabulary.Admin.Meanings;

public sealed record AdminVocabularyMeaningResponse(
    long Id,
    long VocabularyId,
    string MeaningVi,
    short SenseOrder,
    string? UsageNoteVi,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
