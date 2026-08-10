namespace HanYu.Domain.Constants;

public static class SystemPermissions
{
    // Cấu trúc chuẩn: Resource.Action

    // Quản lý người dùng
    public const string UsersRead = "users.read";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";

    // Quản lý vai trò
    public const string RolesRead = "roles.read";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";

    // Quản lý khoá học / bài giảng
    public const string CoursesRead = "courses.read";
    public const string CoursesCreate = "courses.create";
    public const string CoursesUpdate = "courses.update";
    public const string CoursesDelete = "courses.delete";

    public static IReadOnlyList<PermissionDefinition> All => new List<PermissionDefinition>
    {
        new(UsersRead, "Xem danh sách và chi tiết người dùng", "users", "read"),
        new(UsersCreate, "Tạo mới người dùng", "users", "create"),
        new(UsersUpdate, "Cập nhật thông tin người dùng", "users", "update"),
        new(UsersDelete, "Xóa người dùng", "users", "delete"),

        new(RolesRead, "Xem danh sách và chi tiết vai trò", "roles", "read"),
        new(RolesCreate, "Tạo mới vai trò", "roles", "create"),
        new(RolesUpdate, "Cập nhật thông tin vai trò", "roles", "update"),
        new(RolesDelete, "Xóa vai trò", "roles", "delete"),

        new(CoursesRead, "Xem danh sách khóa học", "courses", "read"),
        new(CoursesCreate, "Tạo mới khóa học", "courses", "create"),
        new(CoursesUpdate, "Cập nhật khóa học", "courses", "update"),
        new(CoursesDelete, "Xóa khóa học", "courses", "delete")
    };
}

public sealed record PermissionDefinition(string Code, string Description, string Resource, string Action);
