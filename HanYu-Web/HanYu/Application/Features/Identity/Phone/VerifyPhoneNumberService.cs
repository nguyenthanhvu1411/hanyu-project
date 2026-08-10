using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Phone;

public sealed class VerifyPhoneNumberService
{
    private readonly IPhoneService _service;

    public VerifyPhoneNumberService(
        IPhoneService service)
    {
        _service = service;
    }

    public Task<Result<PhoneResponse>> ExecuteAsync(
        Guid userId,
        VerifyPhoneNumberRequest request,
        CancellationToken cancellationToken = default)
    {
        return _service.VerifyAsync(
            userId,
            request.Code,
            cancellationToken);
    }
}
