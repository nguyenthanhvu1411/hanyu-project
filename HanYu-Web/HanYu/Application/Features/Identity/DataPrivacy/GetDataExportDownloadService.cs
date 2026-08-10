using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class GetDataExportDownloadService
{
    private readonly IDataPrivacyService _service;

    public GetDataExportDownloadService(
        IDataPrivacyService service)
    {
        _service = service;
    }

    public Task<
        Result<DataExportDownloadResponse>>
        ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        return _service
            .GetLatestExportDownloadAsync(
                userId,
                cancellationToken);
    }
}
