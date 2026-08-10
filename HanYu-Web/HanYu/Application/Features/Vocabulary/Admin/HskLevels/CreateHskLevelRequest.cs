namespace HanYu.Application.Features.Vocabulary.Admin.HskLevels;

public sealed record CreateHskLevelRequest(

    string Code,
    string NameVi,
    int SortOrder);
