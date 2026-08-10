namespace HanYu.Application.Features.Vocabulary.Admin.Meanings;

public sealed record UpdateVocabularyMeaningRequest(
    string MeaningVi,
    short SenseOrder,
    string? UsageNoteVi);
