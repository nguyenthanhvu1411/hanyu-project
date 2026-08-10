namespace HanYu.Application.Features.Identity.Logout;

public sealed record LogoutRequest(
    string RefreshToken);