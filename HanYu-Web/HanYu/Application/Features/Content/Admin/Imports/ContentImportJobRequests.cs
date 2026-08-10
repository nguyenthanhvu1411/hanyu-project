using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Content.Admin.Imports;

public sealed record CreateContentImportJobRequest(
    ContentImportType ImportType,
    string OriginalFileName,
    string StoragePath);

public sealed record UpdateContentImportSourceRequest(
    string OriginalFileName,
    string StoragePath);
