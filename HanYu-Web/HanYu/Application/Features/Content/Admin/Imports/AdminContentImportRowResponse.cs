namespace HanYu.Application.Features.Content.Admin.Imports;

public sealed record AdminContentImportRowResponse(
    long Id,
    int RowNumber,
    string SourceJson,
    bool IsSuccessful,
    long? CreatedEntityId,
    string? ErrorCode,
    string? ErrorMessage,
    DateTimeOffset ProcessedAt);
