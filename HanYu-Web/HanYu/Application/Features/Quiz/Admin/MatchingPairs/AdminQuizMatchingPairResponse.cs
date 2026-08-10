namespace HanYu.Application.Features.Quiz.Admin.MatchingPairs;

public sealed record AdminQuizMatchingPairResponse(
    long Id,
    Guid PublicId,
    long QuestionId,
    string LeftText,
    string RightText,
    string? LeftPinyin,
    string? RightPinyin,
    short SortOrder);
