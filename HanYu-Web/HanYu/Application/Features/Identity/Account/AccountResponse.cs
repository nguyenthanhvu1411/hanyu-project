namespace HanYu.Application.Features.Identity.Account;

public sealed record AccountResponse(
    Guid PublicId,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);
