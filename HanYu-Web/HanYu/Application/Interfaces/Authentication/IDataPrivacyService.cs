using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.DataPrivacy;

namespace HanYu.Application.Interfaces.Authentication;

public interface IDataPrivacyService
{
    Task<Result<IReadOnlyCollection<ConsentResponse>>>
        GetConsentsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<ConsentResponse>> UpdateConsentAsync(
        Guid userId,
        UpdateConsentRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<DataExportResponse>> RequestExportAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<DataExportResponse>>>
        GetExportsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<Result<DataExportDownloadResponse>>
        GetLatestExportDownloadAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
