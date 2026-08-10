using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Relations;

public sealed record CreateVocabularyRelationRequest(
    long RelatedVocabularyId,
    VocabularyRelationType RelationType,
    string? NoteVi);
