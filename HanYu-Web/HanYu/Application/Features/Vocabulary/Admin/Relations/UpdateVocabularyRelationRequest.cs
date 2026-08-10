using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Relations;

public sealed record UpdateVocabularyRelationRequest(
    VocabularyRelationType RelationType,
    string? NoteVi);
