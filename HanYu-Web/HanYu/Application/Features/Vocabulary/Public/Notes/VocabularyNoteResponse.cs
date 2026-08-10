namespace HanYu.Application.Features.Vocabulary.Public.Notes;

public sealed record VocabularyNoteResponse(
    string Content,
    bool IsPinned,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
