using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Vocabulary.Admin.Vocabulary;

public sealed record AdminVocabularyQuery
    : PaginationRequest
{
    public string? Q { get; init; }

    public long? HskLevelId { get; init; }

    public long? PartOfSpeechId { get; init; }

    public long? TopicId { get; init; }

    public short? Difficulty { get; init; }

    public ContentStatus? Status { get; init; }

    public string? Sort { get; init; }
        = "-updatedAt";
}
