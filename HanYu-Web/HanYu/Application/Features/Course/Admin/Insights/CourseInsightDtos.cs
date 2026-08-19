namespace HanYu.Application.Features.Course.Admin.Insights;

public sealed record CourseHistoryItemDto(
    long? Id,
    string Action,
    string Label,
    Guid? UserId,
    string? UserDisplayName,
    string? OldValuesJson,
    string? NewValuesJson,
    string? ChangedPropertiesJson,
    string? IpAddress,
    string? CorrelationId,
    DateTimeOffset OccurredAt);

public sealed record CourseStatisticsDto(
    long CourseId,
    int TotalChapters,
    int ActiveChapters,
    int TotalLessons,
    int TotalStudents,
    int StudentsInProgress,
    int StudentsCompleted,
    decimal AverageCompletionPercent,
    int? EstimatedMinutes);

public sealed record CourseStudentDto(
    Guid UserId,
    string Email,
    string DisplayName,
    int StartedLessons,
    int CompletedLessons,
    int TotalLessons,
    decimal CompletionPercent,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? LastAccessedAt,
    DateTimeOffset? CompletedAt);

public sealed class CourseStudentsQuery
{
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
}
