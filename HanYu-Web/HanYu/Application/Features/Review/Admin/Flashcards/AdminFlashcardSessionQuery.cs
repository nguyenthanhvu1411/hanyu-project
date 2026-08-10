using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Flashcards;

public sealed record AdminFlashcardSessionQuery : PaginationRequest
{
    public Guid? UserId { get; init; }
    public FlashcardMode? Mode { get; init; }
    public FlashcardSourceType? SourceType { get; init; }
    public FlashcardSessionStatus? Status { get; init; }
    public long? SourceId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public string? Sort { get; init; } = "-startedAt";
}
