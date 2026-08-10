namespace HanYu.Application.Features.Vocabulary.Admin.Topics;

public sealed record CreateTopicRequest(
    string Slug,
    string NameVi,
    string? DescriptionVi,
    int SortOrder);
