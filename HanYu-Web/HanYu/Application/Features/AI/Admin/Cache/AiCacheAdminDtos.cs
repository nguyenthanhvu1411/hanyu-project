using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.AI.Admin.Cache;

public sealed record AdminAiCacheQuery : PaginationRequest
{
    public AiFeatureType? FeatureType { get; init; }

    public string? Model { get; init; }

    public bool? Expired { get; init; }

    public string? Sort { get; init; }
        = "-updatedAt";
}

public sealed record AdminAiCacheResponse(
    long Id,
    Guid PublicId,
    AiFeatureType FeatureType,
    string CacheKey,
    string Model,
    string PromptVersion,
    int HitCount,
    DateTimeOffset? LastAccessedAt,
    DateTimeOffset? ExpiresAt,
    bool IsExpired,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
