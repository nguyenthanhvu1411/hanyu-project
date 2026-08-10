using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed record DataExportResponse(
    DataExportStatus Status,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ExpiresAt,
    bool CanDownload,
    string? ErrorMessage);
