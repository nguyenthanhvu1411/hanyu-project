namespace HanYu.Application.Features.Quiz.Admin.Options;

public sealed record AdminQuizQuestionOptionResponse(
    long Id,
    Guid PublicId,
    long QuestionId,
    string OptionText,
    string? OptionPinyin,
    bool IsCorrect,
    short SortOrder,
    string? ExplanationVi);
