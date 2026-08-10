using System.Net;
using System.Text.Json;
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
        var draft =
            await CreateDraftLessonAsync();

        var response =
            await Factory.CreateAnonymousClient()
                .GetAsync(
                    $"/api/v1/public/lessons/{draft.PublicId}");

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
    public async Task Admin_CanCreateUpdateAndDeleteSection()
    {
        var adminId = await CreateUserAsync("admin-section");
        var lesson = await CreateDraftLessonAsync();
        var client = Factory.CreateAdminClient(adminId);

        var create = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/sections",
            new
            {
                sectionType = 2,
                sortOrder = 1,
                titleVi = "Giải thích ban đầu",
                contentVi = "Nội dung section ban đầu",
                isRequired = true,
                estimatedSeconds = 180
            });

        await AssertSuccessAsync(create);
        var sectionId = await ReadLongPropertyAsync(create, "id");

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/sections/{sectionId}",
            new
            {
                sectionType = 4,
                sortOrder = 2,
                titleVi = "Ngữ pháp cập nhật",
                contentVi = "Nội dung đã cập nhật",
                isRequired = false,
                estimatedSeconds = 240
            });

        await AssertSuccessAsync(update);

        var listAfterUpdate = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/sections");
        await AssertSuccessAsync(listAfterUpdate);
        (await listAfterUpdate.Content.ReadAsStringAsync())
            .Should().Contain("Ngữ pháp cập nhật");

        var delete = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/sections/{sectionId}");
        await AssertSuccessAsync(delete);

        var listAfterDelete = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/sections");
        await AssertSuccessAsync(listAfterDelete);
        (await listAfterDelete.Content.ReadAsStringAsync())
            .Should().NotContain("Ngữ pháp cập nhật");
    }

    [Fact]
    public async Task Admin_CanAttachUpdateAndDetachVocabulary()
    {
        var adminId = await CreateUserAsync("admin-vocabulary");
        var lesson = await CreateDraftLessonAsync();
        var learningData = await TestDataSeeder.SeedLearningDataAsync(Factory);
        var client = Factory.CreateAdminClient(adminId);

        var attach = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/vocabulary",
            new
            {
                vocabularyId = learningData.VocabularyId,
                sortOrder = 3,
                isRequired = true
            });
        await AssertSuccessAsync(attach);

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/vocabulary/{learningData.VocabularyId}",
            new
            {
                sortOrder = 5,
                isRequired = false
            });
        await AssertSuccessAsync(update);

        var listAfterUpdate = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/vocabulary");
        await AssertSuccessAsync(listAfterUpdate);
        var updatedBody = await listAfterUpdate.Content.ReadAsStringAsync();
        updatedBody.Should().Contain(learningData.Simplified);
        updatedBody.Should().Contain("\"sortOrder\":5");
        updatedBody.Should().Contain("\"isRequired\":false");

        var detach = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/vocabulary/{learningData.VocabularyId}");
        await AssertSuccessAsync(detach);

        var listAfterDetach = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/vocabulary");
        await AssertSuccessAsync(listAfterDetach);
        (await listAfterDetach.Content.ReadAsStringAsync())
            .Should().NotContain(learningData.Simplified);
    }

    [Fact]
    public async Task Admin_CanCreateUpdateAndDeleteAsset()
    {
        var adminId = await CreateUserAsync("admin-asset");
        var lesson = await CreateDraftLessonAsync();
        var client = Factory.CreateAdminClient(adminId);

        var create = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/assets",
            new
            {
                assetType = 0,
                url = "https://example.com/lesson-cover.png",
                captionVi = "Ảnh minh họa ban đầu",
                audioAssetId = (long?)null,
                sortOrder = 0
            });
        await AssertSuccessAsync(create);
        var assetId = await ReadLongPropertyAsync(create, "id");

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/assets/{assetId}",
            new
            {
                url = "https://example.com/lesson-cover-v2.png",
                captionVi = "Ảnh minh họa cập nhật",
                audioAssetId = (long?)null,
                sortOrder = 2
            });
        await AssertSuccessAsync(update);

        var listAfterUpdate = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/assets");
        await AssertSuccessAsync(listAfterUpdate);
        (await listAfterUpdate.Content.ReadAsStringAsync())
            .Should().Contain("Ảnh minh họa cập nhật");

        var delete = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/assets/{assetId}");
        await AssertSuccessAsync(delete);

        var listAfterDelete = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/assets");
        await AssertSuccessAsync(listAfterDelete);
        (await listAfterDelete.Content.ReadAsStringAsync())
            .Should().NotContain("Ảnh minh họa cập nhật");
    }

    [Fact]
    public async Task Admin_CanAddAndRemovePrerequisite()
    {
        var adminId = await CreateUserAsync("admin-prerequisite");
        var lesson = await CreateDraftLessonAsync("lesson-target");
        var required = await CreateDraftLessonAsync("lesson-required");
        var client = Factory.CreateAdminClient(adminId);

        var add = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/prerequisites",
            new
            {
                requiredLessonId = required.Id
            });
        await AssertSuccessAsync(add);

        var listAfterAdd = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/prerequisites");
        await AssertSuccessAsync(listAfterAdd);
        var afterAddBody = await listAfterAdd.Content.ReadAsStringAsync();
        afterAddBody.Should().Contain(required.PublicId.ToString());

        var remove = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/prerequisites/{required.Id}");
        await AssertSuccessAsync(remove);

        var listAfterRemove = await client.GetAsync(
            $"/api/v1/admin/lessons/{lesson.Id}/prerequisites");
        await AssertSuccessAsync(listAfterRemove);
        (await listAfterRemove.Content.ReadAsStringAsync())
            .Should().NotContain(required.PublicId.ToString());
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

    private async Task<(long Id, Guid PublicId)> CreateDraftLessonAsync(
        string prefix = "draft-lesson")
    {
        return await Factory.ExecuteDbAsync(
            async db =>
            {
                var hskLevelId =
                    await db.Set<HanYu.Domain.Entities.Vocabulary.HskLevel>()
                        .Where(x => x.Code == "HSK1")
                        .Select(x => x.Id)
                        .SingleAsync();

                var lesson =
                    new Domain.Entities.Lesson.Lesson(
                        hskLevelId,
                        Unique(prefix),
                        $"{prefix} integration");

                db.Add(lesson);
                await db.SaveChangesAsync();

                return (lesson.Id, lesson.PublicId);
            });
    }

    private static async Task<long> ReadLongPropertyAsync(
        HttpResponseMessage response,
        string propertyName)
    {
        var json =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty(propertyName)
            .GetInt64();
    }

    private static async Task AssertSuccessAsync(
        HttpResponseMessage response)
    {
        var body =
            await response.Content.ReadAsStringAsync();

        response.IsSuccessStatusCode
            .Should()
            .BeTrue(
                $"HTTP {(int)response.StatusCode} {response.StatusCode}; body: {body}");
    }
}
