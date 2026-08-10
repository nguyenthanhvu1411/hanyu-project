using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.States;

public sealed record AdminVocabularyStateQuery : PaginationRequest
{
    public Guid? UserId { get; init; }
    public long? VocabularyId { get; init; }
    public string? Q { get; init; }
    public long? HskLevelId { get; init; }
    public long? TopicId { get; init; }
    public LearningState? LearningState { get; init; }
    public bool? IsFavorite { get; init; }
    public bool? IsDue { get; init; }
    public bool? IsOverdue { get; init; }
    public decimal? MinMastery { get; init; }
    public decimal? MaxMastery { get; init; }
    public string? Sort { get; init; } = "nextReviewAt";
}
