namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed record DataExportDownloadResponse(
    string DownloadUrl,
    DateTimeOffset UrlExpiresAt);
