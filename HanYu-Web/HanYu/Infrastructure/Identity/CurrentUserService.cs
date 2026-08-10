using System.Security.Claims;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;

namespace HanYu.Infrastructure.Identity;

public sealed class CurrentUserService
    : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var value =
                User?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                value,
                out var userId)
                ? userId
                : null;
        }
    }

    public Guid? SessionKey
    {
        get
        {
            var value =
                User?.FindFirstValue("sid");

            return Guid.TryParse(
                value,
                out var sessionKey)
                ? sessionKey
                : null;
        }
    }

    public string? Email =>
        User?.FindFirstValue(
            ClaimTypes.Email);

    public IReadOnlyCollection<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) == true;
    }
}