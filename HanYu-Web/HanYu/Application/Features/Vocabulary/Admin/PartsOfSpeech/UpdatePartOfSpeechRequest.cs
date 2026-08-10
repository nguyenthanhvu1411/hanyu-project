namespace HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;

public sealed record UpdatePartOfSpeechRequest(
    string Code,
    string NameVi,
    string? NameEn);
