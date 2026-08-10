namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyMeaningResponse(
    string MeaningVi,
    short SenseOrder,
    string? UsageNoteVi);
