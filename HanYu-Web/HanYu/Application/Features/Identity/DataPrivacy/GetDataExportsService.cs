using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class GetDataExportsService
{
    private readonly IDataPrivacyService _service;

    public GetDataExportsService(
        IDataPrivacyService service)
    {
        _service = service;
    }

    public Task<Result<
        IReadOnlyCollection<DataExportResponse>>>
        ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        return _service.GetExportsAsync(
            userId,
            cancellationToken);
    }
}
