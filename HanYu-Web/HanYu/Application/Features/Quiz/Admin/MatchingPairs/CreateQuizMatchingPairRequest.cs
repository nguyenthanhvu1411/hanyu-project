namespace HanYu.Application.Features.Quiz.Admin.MatchingPairs;

public sealed record CreateQuizMatchingPairRequest(
    string LeftText,
    string RightText,
    string? LeftPinyin,
    string? RightPinyin,
    short SortOrder);
