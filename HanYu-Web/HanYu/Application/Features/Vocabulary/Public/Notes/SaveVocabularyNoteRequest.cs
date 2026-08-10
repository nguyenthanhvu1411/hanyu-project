namespace HanYu.Application.Features.Vocabulary.Public.Notes;

public sealed record SaveVocabularyNoteRequest(
    string Content,
    bool IsPinned);
