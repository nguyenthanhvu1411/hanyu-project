using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyRelationResponse(
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    VocabularyRelationType RelationType,
    string? NoteVi);
