namespace HanYu.Application.Features.Review.Public.Queue;

public sealed record ReviewQueueQuery
{
    public int Limit { get; init; } = 20;

    public bool IncludeNew { get; init; }

    public long? HskLevel { get; init; }

    public string? Topic { get; init; }
}
