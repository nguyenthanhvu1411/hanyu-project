namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public sealed record RoleSeedItem(
    string Code,
    string Name,
    string Description,
    IReadOnlyCollection<string> Permissions);
