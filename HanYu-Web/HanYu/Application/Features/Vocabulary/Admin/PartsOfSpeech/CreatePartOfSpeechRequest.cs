namespace HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;

public sealed record CreatePartOfSpeechRequest(
    string Code,
    string NameVi,
    string? NameEn);
