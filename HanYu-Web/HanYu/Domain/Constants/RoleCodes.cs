namespace HanYu.Domain.Constants;

public static class RoleCodes
{
    public const string SuperAdmin =
        "SUPER_ADMIN";

    public const string Admin =
        "ADMIN";

    public const string ContentManager =
        "CONTENT_MANAGER";

    public const string ContentEditor =
        "CONTENT_EDITOR";

    public const string Reviewer =
        "REVIEWER";

    public const string Teacher =
        "TEACHER";

    public const string Support =
        "SUPPORT";

    public const string User =
        "USER";

    public static readonly IReadOnlyCollection<string> All =
    [
        SuperAdmin,
        Admin,
        ContentManager,
        ContentEditor,
        Reviewer,
        Teacher,
        Support,
        User
    ];
}
