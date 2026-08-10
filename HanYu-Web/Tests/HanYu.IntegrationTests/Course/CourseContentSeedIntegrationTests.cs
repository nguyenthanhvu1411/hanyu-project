using HanYu.Application.Interfaces.Storage;
using HanYu.Infrastructure.Persistence;
using HanYu.Infrastructure.Persistence.Seeding.Content;
using HanYu.Infrastructure.Storage;
using HanYu.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace HanYu.IntegrationTests.Course;

public sealed class CourseContentSeedIntegrationTests : IntegrationTestBase
{
    public CourseContentSeedIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Seed_CreatesSixCourses_WithStorageBackedCoverUrls_AndIsIdempotent()
    {
        await Factory.ResetDatabaseAsync();
        await TestDataSeeder.SeedReferenceDataAsync(Factory);

        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HanYuDbContext>();
        var storage = scope.ServiceProvider.GetRequiredService<IPublicFileStorage>();

        var seeder = new CourseContentSeeder(
            db,
            storage,
            Options.Create(new ContentSeedOptions { Enabled = true }),
            Options.Create(new StorageOptions
            {
                PublicBucketName = "hanyu-public",
                PublicBaseUrl = "https://storage.test/hanyu-public"
            }),
            NullLogger<CourseContentSeeder>.Instance);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var courses = await db.Courses
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        courses.Should().HaveCount(6);
        courses.Select(x => x.Code).Should().Equal(
            "COURSE-HSK1",
            "COURSE-HSK2",
            "COURSE-HSK3",
            "COURSE-HSK4",
            "COURSE-HSK5",
            "COURSE-HSK6");

        courses.Should().OnlyContain(x =>
            x.CoverImageUrl != null &&
            x.CoverImageUrl.StartsWith("https://storage.test/", StringComparison.Ordinal) &&
            !x.CoverImageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase));
    }
}
