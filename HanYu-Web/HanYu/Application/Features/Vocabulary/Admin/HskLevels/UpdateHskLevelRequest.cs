namespace HanYu.Application.Features.Vocabulary.Admin.HskLevels;

public sealed record UpdateHskLevelRequest(
    string Code,
    string NameVi,
    int SortOrder);
