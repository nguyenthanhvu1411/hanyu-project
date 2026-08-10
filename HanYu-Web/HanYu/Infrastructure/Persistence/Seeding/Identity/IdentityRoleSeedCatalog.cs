using HanYu.Domain.Constants;

namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public static class IdentityRoleSeedCatalog
{
    public static IReadOnlyCollection<RoleSeedItem> All =>
        Build();

    private static IReadOnlyCollection<RoleSeedItem> Build()
    {
        var permissionCatalog =
            IdentityPermissionSeedCatalog.All;

        var allPermissions =
            permissionCatalog
                .Select(x => x.Code)
                .ToArray();

        return
        [
            new RoleSeedItem(
                RoleCodes.SuperAdmin,
                "Super Administrator",
                "Toàn quyền quản trị hệ thống HanYu.",
                allPermissions),

            new RoleSeedItem(
                RoleCodes.Admin,
                "Administrator",
                "Quản trị hệ thống.",
                GetAdminPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.ContentManager,
                "Quản lý nội dung",
                "Quản lý toàn bộ nội dung học tập.",
                GetContentManagerPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.ContentEditor,
                "Biên tập nội dung",
                "Tạo và chỉnh sửa nội dung.",
                GetContentEditorPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.Reviewer,
                "Kiểm duyệt viên",
                "Kiểm tra, duyệt và phê duyệt nội dung.",
                GetReviewerPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.Teacher,
                "Giáo viên",
                "Theo dõi học viên và nội dung học tập.",
                GetTeacherPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.Support,
                "Hỗ trợ người dùng",
                "Hỗ trợ tài khoản, phiên đăng nhập và tiến độ.",
                GetSupportPermissions(
                    permissionCatalog)),

            new RoleSeedItem(
                RoleCodes.User,
                "Người dùng",
                "Người học thông thường.",
                [])
        ];
    }

    private static string[] GetAdminPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        var excludedResources = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "audit-logs",
            "permissions",
        };

        var excludedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "roles.update",
            "roles.delete",
            "roles.restore",
            "roles.permissions.manage",
            "users.roles.manage"
        };

        return permissions
            .Where(x => !excludedResources.Contains(x.Resource))
            .Where(x => !excludedCodes.Contains(x.Code))
            .Select(x => x.Code)
            .ToArray();
    }

    private static string[] GetContentManagerPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        var resources =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "hsk-levels",
                "courses",
                "chapters",
                "lessons",

                "vocabulary",
                "vocabulary-meanings",
                "vocabulary-examples",
                "vocabulary-relations",
                "vocabulary-topics",
                "parts-of-speech",

                "question-bank",
                "quizzes",
                "quiz-results",

                "media",
                "review-queue",

                "import-jobs",
                "export-jobs"
            };

        return permissions
            .Where(x =>
                resources.Contains(
                    x.Resource))
            .Select(x =>
                x.Code)
            .ToArray();
    }

    private static string[] GetContentEditorPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        var resources =
            ContentResources();

        var actions =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "read",
                "create",
                "update",
                "restore",
                "reorder",
                "submit-review",
                "import",
                "export",
                "upload"
            };

        return permissions
            .Where(x =>
                resources.Contains(
                    x.Resource))
            .Where(x =>
                actions.Contains(
                    x.Action))
            .Select(x =>
                x.Code)
            .ToArray();
    }

    private static string[] GetReviewerPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        var resources =
            ContentResources();

        resources.Add(
            "review-queue");

        var actions =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                "read",
                "review",
                "approve",
                "reject"
            };

        return permissions
            .Where(x =>
                resources.Contains(
                    x.Resource))
            .Where(x =>
                actions.Contains(
                    x.Action))
            .Select(x =>
                x.Code)
            .ToArray();
    }

    private static string[] GetTeacherPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        return ByCodes(
            permissions,

            "users.read",

            "courses.read",
            "chapters.read",
            "lessons.read",

            "vocabulary.read",

            "question-bank.read",

            "quizzes.read",
            "quiz-results.read",

            "learning-goals.read",
            "learning-activities.read",

            "learning-progress.read",
            "learning-progress.export",

            "media.read",
            "media.upload");
    }

    private static string[] GetSupportPermissions(
        IReadOnlyCollection<PermissionSeedItem> permissions)
    {
        return ByCodes(
            permissions,

            "users.read",
            "users.update",
            "users.lock",
            "users.unlock",

            "sessions.read",
            "sessions.revoke",
            "sessions.revoke-all",

            "learning-progress.read",
            "learning-activities.read",

            "audit-logs.read");
    }

    private static string[] ByCodes(
        IReadOnlyCollection<PermissionSeedItem> catalog,
        params string[] codes)
    {
        var valid =
            catalog
                .Select(x => x.Code)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

        return codes
            .Where(
                valid.Contains)
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static HashSet<string> ContentResources()
    {
        return new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
        {
            "hsk-levels",

            "courses",
            "chapters",
            "lessons",

            "vocabulary",
            "vocabulary-meanings",
            "vocabulary-examples",
            "vocabulary-relations",
            "vocabulary-topics",
            "parts-of-speech",

            "question-bank",
            "quizzes",

            "media"
        };
    }
}
