namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed record LessonValidationResultDto(
    bool IsValid,
    IReadOnlyList<LessonValidationIssueDto> Issues)
{
    public IReadOnlyList<string> Errors => Issues
        .Where(issue => issue.Severity == LessonValidationSeverity.Error)
        .Select(issue => issue.Message)
        .ToArray();

    public IReadOnlyList<string> Warnings => Issues
        .Where(issue => issue.Severity == LessonValidationSeverity.Warning)
        .Select(issue => issue.Message)
        .ToArray();
}

public sealed record LessonValidationIssueDto(
    string Code,
    string Message,
    string? Field = null,
    string Severity = LessonValidationSeverity.Error);

public static class LessonValidationSeverity
{
    public const string Error = "error";
    public const string Warning = "warning";
}
