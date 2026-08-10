namespace HanYu.Application.Features.Course.Admin;

public sealed class ReorderCourseChaptersRequest
{
    public IReadOnlyList<ReorderCourseChapterItemRequest> Items { get; init; }
        = [];
}

public sealed class ReorderCourseChapterItemRequest
{
    public long ChapterId { get; init; }

    public int SortOrder { get; init; }
}
