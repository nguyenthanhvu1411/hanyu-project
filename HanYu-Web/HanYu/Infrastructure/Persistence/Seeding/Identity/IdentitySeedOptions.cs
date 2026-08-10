namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public sealed class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public bool Enabled { get; set; } = true;

    public string SuperAdminUserName { get; set; }
        = "superadmin";

    public string SuperAdminEmail { get; set; }
        = string.Empty;

    public string SuperAdminPassword { get; set; }
        = string.Empty;

    public string SuperAdminDisplayName { get; set; }
        = "HanYu Super Administrator";
}
