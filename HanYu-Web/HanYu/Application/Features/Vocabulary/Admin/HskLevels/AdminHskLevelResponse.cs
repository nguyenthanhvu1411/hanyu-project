namespace HanYu.Application.Features.Vocabulary.Admin.HskLevels;

public sealed record AdminHskLevelResponse(
    long Id,
    string Code,
    string NameVi,
    int SortOrder,
    bool IsActive);
