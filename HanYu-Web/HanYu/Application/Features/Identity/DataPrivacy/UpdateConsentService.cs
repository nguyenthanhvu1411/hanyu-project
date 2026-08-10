using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class UpdateConsentService
{
    private readonly IDataPrivacyService _service;

    public UpdateConsentService(
        IDataPrivacyService service)
    {
        _service = service;
    }

    public Task<Result<ConsentResponse>> ExecuteAsync(
        Guid userId,
        UpdateConsentRequest request,
        CancellationToken cancellationToken = default)
    {
        return _service.UpdateConsentAsync(
            userId,
            request,
            cancellationToken);
    }
}
