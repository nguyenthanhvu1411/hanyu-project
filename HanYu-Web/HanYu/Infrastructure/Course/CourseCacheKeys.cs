namespace HanYu.Infrastructure.Course;

public static class CourseCacheKeys
{
    public const string Version =
        "course:public:version";

    public static string List(
        string version,
        string queryKey)
        => $"course:public:{version}:list:{queryKey}";

    public static string Detail(
        string version,
        Guid publicId)
        => $"course:public:{version}:detail:{publicId:N}";

    public static string Slug(
        string version,
        string slug)
        => $"course:public:{version}:slug:{slug}";

    public static string Curriculum(
        string version,
        Guid publicId)
        => $"course:public:{version}:curriculum:{publicId:N}";
}
