using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Phone;

public sealed class UpdatePhoneNumberService
{
    private readonly IPhoneService _service;

    public UpdatePhoneNumberService(
        IPhoneService service)
    {
        _service = service;
    }

    public Task<Result<PhoneResponse>> ExecuteAsync(
        Guid userId,
        UpdatePhoneNumberRequest request,
        CancellationToken cancellationToken = default)
    {
        return _service.UpdateAsync(
            userId,
            request,
            cancellationToken);
    }
}
