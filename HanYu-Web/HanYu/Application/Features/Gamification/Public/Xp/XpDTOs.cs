using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Gamification.Public.Xp;

public sealed record XpHistoryQuery : PaginationRequest
{
    public string? SourceType { get; init; }

    public string? Sort { get; init; } = "-createdAt";
}

public sealed record XpTransactionResponse(
    Guid PublicId,
    int Amount,
    string Reason,
    string? SourceType,
    string? SourceId,
    DateTimeOffset CreatedAt);
