using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Gamification.Admin.Xp;

public sealed record AdminXpQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public string? SourceType { get; init; }

    public bool? IsCredit { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; }
        = "-createdAt";
}

public sealed record AdminXpTransactionResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    int Amount,
    string Reason,
    string? SourceType,
    string? SourceId,
    DateTimeOffset CreatedAt);

public sealed record AdjustXpRequest(
    int Amount,
    string Reason);
