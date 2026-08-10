using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Relations;

public sealed record AdminVocabularyRelationResponse(
    long Id,
    long VocabularyId,
    long RelatedVocabularyId,
    string RelatedSimplified,
    string RelatedPinyin,
    string RelatedMeaningVi,
    VocabularyRelationType RelationType,
    string? NoteVi,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
