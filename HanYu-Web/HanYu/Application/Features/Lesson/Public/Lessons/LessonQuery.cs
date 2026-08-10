using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Lesson.Public.Lessons;

public sealed record LessonQuery : PaginationRequest
{
    public string? Q { get; init; }

    public long? HskLevel { get; init; }

    public string? Topic { get; init; }

    public short? Difficulty { get; init; }

    public bool? Featured { get; init; }

    public string? Sort { get; init; } = "sortOrder";
}
