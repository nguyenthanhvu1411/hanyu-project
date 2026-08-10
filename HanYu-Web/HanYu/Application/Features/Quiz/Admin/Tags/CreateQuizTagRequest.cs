namespace HanYu.Application.Features.Quiz.Admin.Tags;

public sealed record CreateQuizTagRequest(
    string Slug,
    string Name,
    string? NameVi,
    string? DescriptionVi);
