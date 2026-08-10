namespace HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;

public sealed record AdminPartOfSpeechResponse(
    long Id,
    string Code,
    string NameVi,
    string? NameEn,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
