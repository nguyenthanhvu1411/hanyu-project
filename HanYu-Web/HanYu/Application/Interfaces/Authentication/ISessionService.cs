namespace HanYu.Application.Interfaces.Authentication;

public interface ISessionService
{
    Task RevokeAllAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
}
