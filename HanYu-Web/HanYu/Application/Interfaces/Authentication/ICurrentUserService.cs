namespace HanYu.Application.Interfaces.Authentication;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? SessionKey { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsInRole(string role);
}