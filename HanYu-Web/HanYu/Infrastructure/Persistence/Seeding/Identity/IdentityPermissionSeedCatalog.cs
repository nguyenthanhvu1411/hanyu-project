namespace HanYu.Infrastructure.Persistence.Seeding.Identity;

public static class IdentityPermissionSeedCatalog
{
    public static IReadOnlyCollection<PermissionSeedItem> All { get; }
        = Build();

    private static IReadOnlyCollection<PermissionSeedItem> Build()
    {
        var permissions =
            new List<PermissionSeedItem>();

        // Identity
        AddCrudRestore(permissions, "users", "người dùng");

        Add(
            permissions,
            "users.lock",
            "users",
            "lock",
            "Khóa người dùng");

        Add(
            permissions,
            "users.unlock",
            "users",
            "unlock",
            "Mở khóa người dùng");

        Add(
            permissions,
            "users.roles.manage",
            "users",
            "roles.manage",
            "Quản lý vai trò của người dùng");

        Add(
            permissions,
            "users.import",
            "users",
            "import",
            "Nhập người dùng");

        Add(
            permissions,
            "users.export",
            "users",
            "export",
            "Xuất người dùng");

        AddCrudRestore(permissions, "roles", "vai trò");

        Add(
            permissions,
            "roles.permissions.manage",
            "roles",
            "permissions.manage",
            "Quản lý quyền của vai trò");

        AddCrudRestore(
            permissions,
            "permissions",
            "quyền hạn");

        Add(
            permissions,
            "sessions.read",
            "sessions",
            "read",
            "Xem phiên đăng nhập");

        Add(
            permissions,
            "sessions.revoke",
            "sessions",
            "revoke",
            "Thu hồi phiên đăng nhập");

        Add(
            permissions,
            "sessions.revoke-all",
            "sessions",
            "revoke-all",
            "Thu hồi toàn bộ phiên");

        // Learning content
        AddCrudRestore(
            permissions,
            "hsk-levels",
            "cấp độ HSK");

        AddContentModule(
            permissions,
            "courses",
            "khóa học",
            importExport: true);

        AddContentModule(
            permissions,
            "chapters",
            "chương học",
            importExport: true,
            reorder: true);

        AddContentModule(
            permissions,
            "lessons",
            "bài giảng",
            importExport: true,
            reorder: true);

        // Vocabulary
        AddContentModule(
            permissions,
            "vocabulary",
            "từ vựng",
            importExport: true);

        AddCrud(
            permissions,
            "vocabulary-meanings",
            "nghĩa từ vựng");

        AddCrud(
            permissions,
            "vocabulary-examples",
            "ví dụ từ vựng");

        AddCrud(
            permissions,
            "vocabulary-relations",
            "quan hệ từ vựng");

        AddCrudRestore(
            permissions,
            "vocabulary-topics",
            "chủ đề từ vựng");

        AddCrudRestore(
            permissions,
            "parts-of-speech",
            "loại từ");

        // Quiz
        AddCrudRestore(
            permissions,
            "question-bank",
            "ngân hàng câu hỏi");

        Add(
            permissions,
            "question-bank.review",
            "question-bank",
            "review",
            "Duyệt câu hỏi");

        Add(
            permissions,
            "question-bank.approve",
            "question-bank",
            "approve",
            "Phê duyệt câu hỏi");

        Add(
            permissions,
            "question-bank.reject",
            "question-bank",
            "reject",
            "Từ chối câu hỏi");

        AddImportExport(
            permissions,
            "question-bank",
            "ngân hàng câu hỏi");

        AddCrudRestore(
            permissions,
            "quizzes",
            "bài kiểm tra");

        Add(
            permissions,
            "quizzes.publish",
            "quizzes",
            "publish",
            "Xuất bản bài kiểm tra");

        Add(
            permissions,
            "quizzes.unpublish",
            "quizzes",
            "unpublish",
            "Hủy xuất bản bài kiểm tra");

        AddImportExport(
            permissions,
            "quizzes",
            "bài kiểm tra");

        Add(
            permissions,
            "quiz-results.read",
            "quiz-results",
            "read",
            "Xem kết quả bài kiểm tra");

        Add(
            permissions,
            "quiz-results.delete",
            "quiz-results",
            "delete",
            "Xóa kết quả bài kiểm tra");

        Add(
            permissions,
            "quiz-results.export",
            "quiz-results",
            "export",
            "Xuất kết quả bài kiểm tra");

        // Learning
        AddCrud(
            permissions,
            "learning-goals",
            "mục tiêu học tập");

        Add(
            permissions,
            "learning-activities.read",
            "learning-activities",
            "read",
            "Xem hoạt động học tập");

        Add(
            permissions,
            "learning-activities.export",
            "learning-activities",
            "export",
            "Xuất hoạt động học tập");

        Add(
            permissions,
            "learning-progress.read",
            "learning-progress",
            "read",
            "Xem tiến độ học tập");

        Add(
            permissions,
            "learning-progress.update",
            "learning-progress",
            "update",
            "Điều chỉnh tiến độ học tập");

        Add(
            permissions,
            "learning-progress.reset",
            "learning-progress",
            "reset",
            "Đặt lại tiến độ học tập");

        Add(
            permissions,
            "learning-progress.export",
            "learning-progress",
            "export",
            "Xuất tiến độ học tập");

        // Media
        Add(
            permissions,
            "media.read",
            "media",
            "read",
            "Xem media");

        Add(
            permissions,
            "media.upload",
            "media",
            "upload",
            "Tải media lên");

        Add(
            permissions,
            "media.delete",
            "media",
            "delete",
            "Xóa media");

        Add(
            permissions,
            "media.restore",
            "media",
            "restore",
            "Khôi phục media");

        Add(
            permissions,
            "media.quarantine",
            "media",
            "quarantine",
            "Cách ly media");

        // Notification
        AddCrud(
            permissions,
            "notifications",
            "thông báo");

        Add(
            permissions,
            "notifications.send",
            "notifications",
            "send",
            "Gửi thông báo");

        Add(
            permissions,
            "notifications.broadcast",
            "notifications",
            "broadcast",
            "Gửi thông báo hàng loạt");

        AddCrud(
            permissions,
            "email-templates",
            "mẫu email");

        Add(
            permissions,
            "email-templates.preview",
            "email-templates",
            "preview",
            "Xem trước email");

        Add(
            permissions,
            "email-templates.send-test",
            "email-templates",
            "send-test",
            "Gửi email thử");

        // Governance
        Add(
            permissions,
            "audit-logs.read",
            "audit-logs",
            "read",
            "Xem nhật ký quản trị");

        Add(
            permissions,
            "audit-logs.export",
            "audit-logs",
            "export",
            "Xuất nhật ký quản trị");

        Add(
            permissions,
            "review-queue.read",
            "review-queue",
            "read",
            "Xem hàng đợi duyệt");

        Add(
            permissions,
            "review-queue.review",
            "review-queue",
            "review",
            "Duyệt nội dung");

        Add(
            permissions,
            "review-queue.approve",
            "review-queue",
            "approve",
            "Phê duyệt nội dung");

        Add(
            permissions,
            "review-queue.reject",
            "review-queue",
            "reject",
            "Từ chối nội dung");

        // System
        Add(
            permissions,
            "system-settings.read",
            "system-settings",
            "read",
            "Xem cấu hình hệ thống");

        Add(
            permissions,
            "system-settings.update",
            "system-settings",
            "update",
            "Thay đổi cấu hình hệ thống");

        AddCrud(
            permissions,
            "feature-flags",
            "feature flag");

        // Report
        Add(
            permissions,
            "reports.read",
            "reports",
            "read",
            "Xem báo cáo");

        Add(
            permissions,
            "reports.export",
            "reports",
            "export",
            "Xuất báo cáo");

        // Import / export job
        Add(
            permissions,
            "import-jobs.read",
            "import-jobs",
            "read",
            "Xem job nhập dữ liệu");

        Add(
            permissions,
            "import-jobs.create",
            "import-jobs",
            "create",
            "Tạo job nhập dữ liệu");

        Add(
            permissions,
            "import-jobs.cancel",
            "import-jobs",
            "cancel",
            "Hủy job nhập dữ liệu");

        Add(
            permissions,
            "export-jobs.read",
            "export-jobs",
            "read",
            "Xem job xuất dữ liệu");

        Add(
            permissions,
            "export-jobs.create",
            "export-jobs",
            "create",
            "Tạo job xuất dữ liệu");

        Add(
            permissions,
            "export-jobs.cancel",
            "export-jobs",
            "cancel",
            "Hủy job xuất dữ liệu");

        Add(
            permissions,
            "export-jobs.download",
            "export-jobs",
            "download",
            "Tải file xuất dữ liệu");

        // Community
        Add(
            permissions,
            "comments.read",
            "comments",
            "read",
            "Xem bình luận");

        Add(
            permissions,
            "comments.delete",
            "comments",
            "delete",
            "Xóa bình luận");

        Add(
            permissions,
            "comments.moderate",
            "comments",
            "moderate",
            "Kiểm duyệt bình luận");

        Add(
            permissions,
            "reviews.read",
            "reviews",
            "read",
            "Xem đánh giá");

        Add(
            permissions,
            "reviews.delete",
            "reviews",
            "delete",
            "Xóa đánh giá");

        Add(
            permissions,
            "reviews.moderate",
            "reviews",
            "moderate",
            "Kiểm duyệt đánh giá");

        return permissions
            .GroupBy(x => x.Code)
            .Select(x => x.First())
            .OrderBy(x => x.Resource)
            .ThenBy(x => x.Action)
            .ToArray();
    }

    private static void Add(
        ICollection<PermissionSeedItem> target,
        string code,
        string resource,
        string action,
        string description)
    {
        target.Add(
            new PermissionSeedItem(
                code,
                resource,
                action,
                description));
    }

    private static void AddCrud(
        ICollection<PermissionSeedItem> target,
        string resource,
        string displayName)
    {
        Add(
            target,
            $"{resource}.read",
            resource,
            "read",
            $"Xem {displayName}");

        Add(
            target,
            $"{resource}.create",
            resource,
            "create",
            $"Tạo {displayName}");

        Add(
            target,
            $"{resource}.update",
            resource,
            "update",
            $"Cập nhật {displayName}");

        Add(
            target,
            $"{resource}.delete",
            resource,
            "delete",
            $"Xóa {displayName}");
    }

    private static void AddCrudRestore(
        ICollection<PermissionSeedItem> target,
        string resource,
        string displayName)
    {
        AddCrud(
            target,
            resource,
            displayName);

        Add(
            target,
            $"{resource}.restore",
            resource,
            "restore",
            $"Khôi phục {displayName}");
    }

    private static void AddImportExport(
        ICollection<PermissionSeedItem> target,
        string resource,
        string displayName)
    {
        Add(
            target,
            $"{resource}.import",
            resource,
            "import",
            $"Nhập {displayName}");

        Add(
            target,
            $"{resource}.export",
            resource,
            "export",
            $"Xuất {displayName}");
    }

    private static void AddContentModule(
        ICollection<PermissionSeedItem> target,
        string resource,
        string displayName,
        bool importExport,
        bool reorder = false)
    {
        AddCrudRestore(
            target,
            resource,
            displayName);

        if (reorder)
        {
            Add(
                target,
                $"{resource}.reorder",
                resource,
                "reorder",
                $"Sắp xếp {displayName}");
        }

        Add(
            target,
            $"{resource}.submit-review",
            resource,
            "submit-review",
            $"Gửi {displayName} đi duyệt");

        Add(
            target,
            $"{resource}.review",
            resource,
            "review",
            $"Duyệt {displayName}");

        Add(
            target,
            $"{resource}.approve",
            resource,
            "approve",
            $"Phê duyệt {displayName}");

        Add(
            target,
            $"{resource}.reject",
            resource,
            "reject",
            $"Từ chối {displayName}");

        Add(
            target,
            $"{resource}.publish",
            resource,
            "publish",
            $"Xuất bản {displayName}");

        Add(
            target,
            $"{resource}.unpublish",
            resource,
            "unpublish",
            $"Hủy xuất bản {displayName}");

        Add(
            target,
            $"{resource}.archive",
            resource,
            "archive",
            $"Lưu trữ {displayName}");

        Add(
            target,
            $"{resource}.rollback",
            resource,
            "rollback",
            $"Khôi phục phiên bản {displayName}");

        if (importExport)
        {
            AddImportExport(
                target,
                resource,
                displayName);
        }
    }
}
