namespace HanYu.Application.Features.Quiz.Admin.Options;

public sealed record UpdateQuizQuestionOptionRequest(
    string OptionText,
    string? OptionPinyin,
    bool IsCorrect,
    short SortOrder,
    string? ExplanationVi);
