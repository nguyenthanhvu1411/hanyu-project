namespace HanYu.Application.Features.Vocabulary.Admin.Meanings;

public sealed record CreateVocabularyMeaningRequest(
    string MeaningVi,
    short SenseOrder,
    string? UsageNoteVi);
