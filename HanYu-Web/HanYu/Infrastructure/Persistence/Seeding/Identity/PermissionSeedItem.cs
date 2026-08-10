namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public sealed record PermissionSeedItem(
    string Code,
    string Resource,
    string Action,
    string Description);
