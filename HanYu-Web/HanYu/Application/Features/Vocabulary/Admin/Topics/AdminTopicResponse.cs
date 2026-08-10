using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Topics;

public sealed record AdminTopicResponse(
    long Id,
    string Slug,
    string NameVi,
    string? DescriptionVi,
    int SortOrder,
    ContentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
