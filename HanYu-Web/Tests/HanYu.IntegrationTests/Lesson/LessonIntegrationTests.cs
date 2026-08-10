using System.Net;
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
    public async Task PublishedLesson_CanBeReadAnonymously()
    {
        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/lessons/{data.LessonPublicId}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task MissingPublicLesson_ReturnsNotFound()
    {
        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/lessons/{Guid.NewGuid()}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DraftLesson_IsNotExposedByPublicEndpoint()
    {
        var hskLevelId =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<HanYu.Domain.Entities.Vocabulary.HskLevel>()
                        .Where(x => x.Code == "HSK1")
                        .Select(x => x.Id)
                        .SingleAsync());

        var draftPublicId =
            await Factory.ExecuteDbAsync(
                async db =>
                {
                    var lesson =
                        new Domain.Entities.Lesson.Lesson(
                            hskLevelId,
                            Unique("draft-lesson"),
                            "Bài giảng nháp");

                    db.Add(lesson);
                    await db.SaveChangesAsync();

                    return lesson.PublicId;
                });

        var response =
            await Factory.CreateAnonymousClient()
                .GetAsync(
                    $"/api/v1/public/lessons/{draftPublicId}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
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
    public async Task Bookmark_CanBeAddedListedAndRemoved()
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

        var add =
            await client.PostAsync(
                $"/api/v1/public/lesson-bookmarks/{data.LessonPublicId}",
                null);

        add.IsSuccessStatusCode.Should().BeTrue();

        var list =
            await client.GetAsync(
                "/api/v1/public/lesson-bookmarks");

        list.IsSuccessStatusCode.Should().BeTrue();

        var listBody =
            await list.Content.ReadAsStringAsync();

        listBody.Should().Contain(data.LessonPublicId.ToString());

        var remove =
            await client.DeleteAsync(
                $"/api/v1/public/lesson-bookmarks/{data.LessonPublicId}");

        remove.IsSuccessStatusCode.Should().BeTrue();

        var afterRemove =
            await client.GetAsync(
                "/api/v1/public/lesson-bookmarks");

        afterRemove.IsSuccessStatusCode.Should().BeTrue();

        var afterRemoveBody =
            await afterRemove.Content.ReadAsStringAsync();

        afterRemoveBody.Should().NotContain(data.LessonPublicId.ToString());
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

    [Fact]
    public async Task Admin_CanReadAndValidateLesson()
    {
        var adminId =
            await CreateUserAsync("admin");

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateAdminClient(
                adminId);

        var detail =
            await client.GetAsync(
                $"/api/v1/admin/lessons/{data.LessonId}");

        var validate =
            await client.GetAsync(
                $"/api/v1/admin/lessons/{data.LessonId}/validate");

        detail.StatusCode.Should().Be(HttpStatusCode.OK);
        validate.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AnonymousUser_CannotAccessAdminLessonApi()
    {
        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var response =
            await Factory.CreateAnonymousClient()
                .GetAsync(
                    $"/api/v1/admin/lessons/{data.LessonId}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}
