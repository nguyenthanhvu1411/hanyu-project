using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Content.Admin.Imports;

public sealed record AdminContentImportJobResponse(
    long Id,
    Guid PublicId,
    ContentImportType ImportType,
    string OriginalFileName,
    string StoragePath,
    ContentImportStatus Status,
    int TotalRows,
    int ProcessedRows,
    int SuccessRows,
    int FailedRows,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string? ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
