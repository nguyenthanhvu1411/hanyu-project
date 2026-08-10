namespace HanYu.Application.Interfaces.Caching;

public interface ICourseCacheInvalidator
{
    Task InvalidatePublicCourseCacheAsync(
        CancellationToken cancellationToken = default);
}
