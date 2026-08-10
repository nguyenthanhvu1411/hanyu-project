using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Course.Admin;

public sealed class AdminCourseQuery
{
    public string? Search { get; init; }

    public long? HskLevelId { get; init; }

    public ContentStatus? Status { get; init; }

    public bool? IsActive { get; init; }

    public bool? IsFeatured { get; init; }

    public bool IncludeDeleted { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
