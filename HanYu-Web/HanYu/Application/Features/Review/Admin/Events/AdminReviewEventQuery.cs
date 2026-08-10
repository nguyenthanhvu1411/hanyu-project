using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Events;

public sealed record AdminReviewEventQuery : PaginationRequest
{
    public Guid? UserId { get; init; }
    public long? VocabularyId { get; init; }
    public long? FlashcardSessionId { get; init; }
    public ReviewRating? Rating { get; init; }
    public bool? WasCorrect { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public decimal? MinMasteryAfter { get; init; }
    public decimal? MaxMasteryAfter { get; init; }
    public string? Sort { get; init; } = "-reviewedAt";
}
