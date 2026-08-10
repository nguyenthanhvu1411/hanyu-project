using HanYu.Application.Features.Identity.Common;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Features.Identity.Mapping;

public static class IdentityMapper
{
    public static CurrentUserResponse ToCurrentUserResponse(
        User user,
        IReadOnlyCollection<string> roles)
    {
        return new CurrentUserResponse(
            user.Id,
            user.PublicId,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            user.Profile?.DisplayName,
            user.Profile?.AvatarUrl,
            roles);
    }
}
