using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Vocabulary.Public.Vocabulary;

public sealed record VocabularyQuery
    : PaginationRequest
{
    public string? Q { get; init; }

    public long? HskLevel { get; init; }

    public string? Topic { get; init; }

    public string? PartOfSpeech { get; init; }

    public short? Difficulty { get; init; }

    public string? Sort { get; init; }
        = "simplified";
}
