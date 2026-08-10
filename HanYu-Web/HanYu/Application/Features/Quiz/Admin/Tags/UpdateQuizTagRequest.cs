namespace HanYu.Application.Features.Quiz.Admin.Tags;

public sealed record UpdateQuizTagRequest(
    string Slug,
    string Name,
    string? NameVi,
    string? DescriptionVi);
