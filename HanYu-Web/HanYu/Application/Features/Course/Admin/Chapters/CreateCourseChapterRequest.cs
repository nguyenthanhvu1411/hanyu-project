namespace HanYu.Application.Features.Course.Admin.Chapters;

public sealed class CreateCourseChapterRequest
{
    public string TitleVi { get; init; } = string.Empty;
    public string? DescriptionVi { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;
}
