using HanYu.Application.Features.Identity.Sessions;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Features.Identity.Mapping;

public static class SessionMapper
{
    public static SessionResponse ToSessionResponse(UserSession session, bool isCurrent)
    {
        return new SessionResponse(
            session.SessionKey,
            session.DeviceName,
            session.DeviceType,
            session.Browser,
            session.OperatingSystem,
            session.IpAddress,
            session.LastActivityAt,
            session.RevokedAt,
            session.Status.ToString(),
            isCurrent);
    }
}
