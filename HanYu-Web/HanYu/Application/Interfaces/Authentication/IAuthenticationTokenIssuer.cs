using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Features.Identity.Login;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Interfaces.Authentication;

public interface IAuthenticationTokenIssuer
{
    Task<Result<AuthResponse>> IssueAsync(
        User user,
        UserSession session,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default);
}
