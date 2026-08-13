using HanYu.Infrastructure.Persistence.Seeding.Content;
using HanYu.Infrastructure.Persistence.Seeding.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanYu.Infrastructure.Persistence;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope =
            services.CreateAsyncScope();

        var provider =
            scope.ServiceProvider;

        var dbContext =
            provider.GetRequiredService<HanYuDbContext>();

        await dbContext.Database
            .MigrateAsync(
                cancellationToken);

        var identitySeeder =
            provider.GetRequiredService<IdentitySeeder>();

        await identitySeeder
            .SeedAsync(
                cancellationToken);

        // Shared Topic taxonomy used by both Vocabulary and Lesson.
        // Create through ActivatorUtilities so the seeder can reuse the current
        // scoped DbContext and logger without requiring another DI registration.
        var taxonomySeeder =
            ActivatorUtilities.CreateInstance<ContentTaxonomySeeder>(
                provider);

        await taxonomySeeder
            .SeedAsync(
                cancellationToken);

        var contentSeeder =
            provider.GetRequiredService<CourseContentSeeder>();

        await contentSeeder
            .SeedAsync(
                cancellationToken);
    }
}
