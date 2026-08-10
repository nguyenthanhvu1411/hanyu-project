using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class RequestDataExportService
{
    private readonly IDataPrivacyService _service;

    public RequestDataExportService(
        IDataPrivacyService service)
    {
        _service = service;
    }

    public Task<Result<DataExportResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _service.RequestExportAsync(
            userId,
            cancellationToken);
    }
}
