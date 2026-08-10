namespace HanYu.Application.Features.Identity.Register;

public sealed record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string DisplayName);