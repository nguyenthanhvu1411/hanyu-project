namespace HanYu.Application.Features.Quiz.Admin.MatchingPairs;

public sealed record UpdateQuizMatchingPairRequest(
    string LeftText,
    string RightText,
    string? LeftPinyin,
    string? RightPinyin,
    short SortOrder);
