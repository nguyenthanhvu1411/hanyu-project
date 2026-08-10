using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed class GetConsentsService
{
    private readonly IDataPrivacyService _service;

    public GetConsentsService(
        IDataPrivacyService service)
    {
        _service = service;
    }

    public Task<Result<
        IReadOnlyCollection<ConsentResponse>>>
        ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        return _service.GetConsentsAsync(
            userId,
            cancellationToken);
    }
}
