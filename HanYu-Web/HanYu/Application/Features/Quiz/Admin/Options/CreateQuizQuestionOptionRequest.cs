namespace HanYu.Application.Features.Quiz.Admin.Options;

public sealed record CreateQuizQuestionOptionRequest(
    string OptionText,
    string? OptionPinyin,
    bool IsCorrect,
    short SortOrder,
    string? ExplanationVi);
