namespace HanYu.Application.Features.Quiz.Admin.Tags;

public sealed record AdminQuizTagResponse(
    long Id,
    Guid PublicId,
    string Slug,
    string Name,
    string? NameVi,
    string? DescriptionVi,
    bool IsActive);
