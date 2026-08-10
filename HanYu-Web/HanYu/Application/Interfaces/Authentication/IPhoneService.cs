using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Phone;

namespace HanYu.Application.Interfaces.Authentication;

public interface IPhoneService
{
    Task<Result<PhoneResponse>> UpdateAsync(
        Guid userId,
        UpdatePhoneNumberRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<PhoneResponse>> VerifyAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);

    Task<Result> SendVerificationAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
