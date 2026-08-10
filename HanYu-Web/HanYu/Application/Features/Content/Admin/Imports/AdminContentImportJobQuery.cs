using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Content.Admin.Imports;

public sealed record AdminContentImportJobQuery : PaginationRequest
{
    public ContentImportType? ImportType { get; init; }

    public ContentImportStatus? Status { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; } = "-createdAt";
}
