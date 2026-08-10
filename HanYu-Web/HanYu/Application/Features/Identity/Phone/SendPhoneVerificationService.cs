using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Phone;

public sealed class SendPhoneVerificationService
{
    private readonly IPhoneService _phoneService;

    public SendPhoneVerificationService(
        IPhoneService phoneService)
    {
        _phoneService = phoneService;
    }

    public Task<Result> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _phoneService.SendVerificationAsync(
            userId,
            cancellationToken);
    }
}
