using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Admin.Feedback;

public sealed record AdminAiFeedbackQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public AiFeedbackRating? Rating { get; init; }

    public string? IssueType { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; }
        = "-createdAt";
}

public sealed record AdminAiFeedbackResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    long AiRequestId,
    AiFeedbackRating Rating,
    string? Comment,
    string? IssueType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
