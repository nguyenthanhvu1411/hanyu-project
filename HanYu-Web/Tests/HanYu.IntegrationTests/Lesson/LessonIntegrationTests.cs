using HanYu.Domain.Entities.Lesson;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Lesson;

using Common;

public sealed class LessonIntegrationTests
    : IntegrationTestBase
{
    public LessonIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task PublishedLesson_CanBeStarted()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        var response =
            await client.PostAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/start",
                null);

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var progressExists =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<UserLessonProgress>()
                        .AnyAsync(
                            x =>
                                x.UserId ==
                                userId &&
                                x.LessonId ==
                                data.LessonId));

        progressExists.Should().BeTrue();
    }

    [Fact]
    public async Task StartTwice_DoesNotDuplicateProgress()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/start",
            null);

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/start",
            null);

        var count =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<UserLessonProgress>()
                        .CountAsync(
                            x =>
                                x.UserId ==
                                userId &&
                                x.LessonId ==
                                data.LessonId));

        count.Should().Be(1);
    }

    [Fact]
    public async Task CompletingRequiredSection_AndLesson_Works()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        await client.PostAsync(
            $"/api/v1/public/lessons/{data.LessonPublicId}/start",
            null);

        var section =
            await client.PutAsJsonAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/sections/{data.SectionPublicId}/progress",
                new
                {
                    timeSpentSeconds = 120,
                    isCompleted = true
                });

        section.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var complete =
            await client.PostAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}/complete",
                null);

        complete.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var completed =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<UserLessonProgress>()
                        .AnyAsync(
                            x =>
                                x.UserId ==
                                userId &&
                                x.LessonId ==
                                data.LessonId &&
                                x.CompletedAt != null));

        completed.Should().BeTrue();
    }
}