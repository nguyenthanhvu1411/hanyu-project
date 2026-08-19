namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record VocabularyValidationResultDto(
    bool IsValid,
    IReadOnlyList<VocabularyValidationIssueDto> Issues)
{
    public IReadOnlyList<string> Errors => Issues
        .Where(issue => issue.Severity == VocabularyValidationSeverity.Error)
        .Select(issue => issue.Message)
        .ToArray();

    public IReadOnlyList<string> Warnings => Issues
        .Where(issue => issue.Severity == VocabularyValidationSeverity.Warning)
        .Select(issue => issue.Message)
        .ToArray();
}

public sealed record VocabularyValidationIssueDto(
    string Code,
    string Message,
    string? Field = null,
    string Severity = VocabularyValidationSeverity.Error);

public static class VocabularyValidationSeverity
{
    public const string Error = "error";
    public const string Warning = "warning";
}
