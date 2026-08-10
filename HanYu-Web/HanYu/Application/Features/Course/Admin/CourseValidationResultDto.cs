namespace HanYu.Application.Features.Course.Admin;

public sealed record CourseValidationResultDto(
    bool IsValid,
    IReadOnlyList<CourseValidationIssueDto> Issues);

public sealed record CourseValidationIssueDto(
    string Code,
    string Message,
    string? Field = null);
