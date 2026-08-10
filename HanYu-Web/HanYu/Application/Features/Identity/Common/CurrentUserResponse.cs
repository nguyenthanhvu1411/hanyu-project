namespace HanYu.Application.Features.Identity.Common;

public sealed record CurrentUserResponse(
    Guid UserId,
    Guid PublicId,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string? DisplayName,
    string? AvatarUrl,
    IReadOnlyCollection<string> Roles);