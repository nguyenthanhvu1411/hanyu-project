using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.AI;

public class AiResponseCache : TimestampedEntity
{
    public AiFeatureType FeatureType { get; private set; }

    public string CacheKey { get; private set; } = string.Empty;

    public string Model { get; private set; } = string.Empty;

    public string PromptVersion { get; private set; } = string.Empty;

    public string ResponseJson { get; private set; } = string.Empty;

    public int HitCount { get; private set; }

    public DateTimeOffset? LastAccessedAt { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    protected AiResponseCache()
    {
    }

    public AiResponseCache(
        AiFeatureType featureType,
        string cacheKey,
        string model,
        string promptVersion,
        string responseJson,
        DateTimeOffset? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(cacheKey))
            throw new ArgumentException(
                "CacheKey không được để trống.",
                nameof(cacheKey));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException(
                "Model không được để trống.",
                nameof(model));

        if (string.IsNullOrWhiteSpace(promptVersion))
            throw new ArgumentException(
                "PromptVersion không được để trống.",
                nameof(promptVersion));

        if (string.IsNullOrWhiteSpace(responseJson))
            throw new ArgumentException(
                "ResponseJson không được để trống.",
                nameof(responseJson));

        FeatureType = featureType;
        CacheKey = cacheKey.Trim();
        Model = model.Trim();
        PromptVersion = promptVersion.Trim();
        ResponseJson = responseJson;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired =>
        ExpiresAt.HasValue &&
        ExpiresAt <= DateTimeOffset.UtcNow;

    public void RegisterHit()
    {
        HitCount++;
        LastAccessedAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void ReplaceResponse(
        string responseJson,
        DateTimeOffset? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(responseJson))
            throw new ArgumentException(
                "ResponseJson không được để trống.",
                nameof(responseJson));

        ResponseJson = responseJson;
        ExpiresAt = expiresAt;

        MarkUpdated();
    }
}
