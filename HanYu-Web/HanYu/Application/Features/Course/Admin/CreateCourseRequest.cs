namespace HanYu.Application.Features.Course.Admin;

public sealed class CreateCourseRequest
{
    public string Code { get; init; }
        = string.Empty;

    public string Slug { get; init; }
        = string.Empty;

    public string TitleVi { get; init; }
        = string.Empty;

    public string? ShortDescriptionVi { get; init; }

    public string? DescriptionVi { get; init; }

    public long? HskLevelId { get; init; }

    public string? CoverImageUrl { get; init; }

    public int SortOrder { get; init; }

    public int? EstimatedMinutes { get; init; }

    public bool IsFeatured { get; init; }
}
