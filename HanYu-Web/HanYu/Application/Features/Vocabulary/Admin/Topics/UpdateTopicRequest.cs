namespace HanYu.Application.Features.Vocabulary.Admin.Topics;

public sealed record UpdateTopicRequest(
    string Slug,
    string NameVi,
    string? DescriptionVi,
    int SortOrder);
