namespace HanYu.Application.Features.Course.Admin;

public sealed class CreateCourseRequest
{
    private string _slug = string.Empty;
    private string _titleVi = string.Empty;

    public string Code { get; init; }
        = string.Empty;

    public string Slug
    {
        get => CourseSlugGenerator.Generate(
            string.IsNullOrWhiteSpace(_slug) ? _titleVi : _slug);
        init => _slug = value ?? string.Empty;
    }

    public string TitleVi
    {
        get => _titleVi;
        init => _titleVi = value ?? string.Empty;
    }

    public string? ShortDescriptionVi { get; init; }

    public string? DescriptionVi { get; init; }

    public long? HskLevelId { get; init; }

    public string? CoverImageUrl { get; init; }

    public int SortOrder { get; init; }

    public int? EstimatedMinutes { get; init; }

    public bool IsFeatured { get; init; }
}
