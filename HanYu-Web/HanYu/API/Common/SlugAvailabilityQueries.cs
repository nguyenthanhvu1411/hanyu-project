using HanYu.Application.Interfaces.Persistence;
using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Common;

public static class SlugAvailabilityQueries
{
    public static async Task<bool> IsCourseSlugAvailableAsync(
        IHanYuDbContext dbContext,
        string slug,
        long? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(slug);
        if (normalized.Length == 0)
        {
            return false;
        }

        return !await dbContext.Courses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                x => x.Slug == normalized &&
                     (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    public static async Task<bool> IsLessonSlugAvailableAsync(
        IHanYuDbContext dbContext,
        string slug,
        long? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(slug);
        if (normalized.Length == 0)
        {
            return false;
        }

        return !await dbContext.Lessons
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                x => x.Slug == normalized &&
                     (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    public static async Task<bool> IsTopicSlugAvailableAsync(
        IHanYuDbContext dbContext,
        string slug,
        long? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(slug);
        if (normalized.Length == 0)
        {
            return false;
        }

        return !await dbContext.Set<Topic>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(
                x => x.Slug == normalized &&
                     (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    public static string Normalize(string slug)
        => string.IsNullOrWhiteSpace(slug)
            ? string.Empty
            : slug.Trim().ToLowerInvariant();
}
