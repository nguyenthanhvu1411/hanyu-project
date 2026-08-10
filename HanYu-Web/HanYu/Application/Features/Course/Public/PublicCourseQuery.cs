namespace HanYu.Application.Features.Course.Public;

public sealed class PublicCourseQuery
{
    public string? Search { get; init; }

    public string? HskCode { get; init; }

    public bool? Featured { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
