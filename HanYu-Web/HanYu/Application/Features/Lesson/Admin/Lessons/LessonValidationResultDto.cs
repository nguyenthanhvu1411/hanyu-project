namespace HanYu.Application.Features.Lesson.Admin.Lessons;

public sealed record LessonValidationResultDto(
    bool IsValid,
    IReadOnlyList<LessonValidationIssueDto> Issues);

public sealed record LessonValidationIssueDto(
    string Code,
    string Message,
    string? Field = null);
