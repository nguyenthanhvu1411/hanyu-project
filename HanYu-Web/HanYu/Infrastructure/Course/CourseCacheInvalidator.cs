using HanYu.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace HanYu.Infrastructure.Course;

public sealed class CourseCacheInvalidator
    : ICourseCacheInvalidator
{
    private readonly IDistributedCache _cache;

    public CourseCacheInvalidator(
        IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task InvalidatePublicCourseCacheAsync(
        CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(
            CourseCacheKeys.Version,
            Guid.NewGuid().ToString("N"),
            cancellationToken);
    }
}
