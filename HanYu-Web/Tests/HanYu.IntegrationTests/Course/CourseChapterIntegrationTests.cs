using System.Net;
using System.Text.Json;
using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Course;

using Common;

public sealed class CourseChapterIntegrationTests : IntegrationTestBase
{
    public CourseChapterIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Admin_CanCreateUpdateDeleteRestoreAndReorderChapters()
    {
        var adminId = await CreateUserAsync("course-admin");
        var seed = await CreateDraftCourseWithLessonsAsync();
        var client = Factory.CreateAdminClient(adminId);

        var firstCreate = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters",
            new
            {
                titleVi = "Chương 1",
                descriptionVi = "Nội dung chương 1",
                sortOrder = 0,
                isActive = true
            });

        firstCreate.StatusCode.Should().Be(HttpStatusCode.OK);
        var first = await ReadObjectAsync(firstCreate);
        var firstId = first.GetProperty("id").GetInt64();
        var firstToken = first.GetProperty("concurrencyToken").GetGuid();

        var secondCreate = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters",
            new
            {
                titleVi = "Chương 2",
                descriptionVi = "Nội dung chương 2",
                sortOrder = 1,
                isActive = true
            });

        secondCreate.StatusCode.Should().Be(HttpStatusCode.OK);
        var second = await ReadObjectAsync(secondCreate);
        var secondId = second.GetProperty("id").GetInt64();

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{firstId}",
            new
            {
                titleVi = "Chương 1 cập nhật",
                descriptionVi = "Đã cập nhật",
                sortOrder = 0,
                isActive = false,
                concurrencyToken = firstToken
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadObjectAsync(update);
        updated.GetProperty("titleVi").GetString().Should().Be("Chương 1 cập nhật");
        updated.GetProperty("isActive").GetBoolean().Should().BeFalse();

        var reorder = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/order",
            new
            {
                items = new[]
                {
                    new { chapterId = firstId, sortOrder = 1 },
                    new { chapterId = secondId, sortOrder = 0 }
                }
            });

        reorder.IsSuccessStatusCode.Should().BeTrue();

        var orderedResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters?includeDeleted=false");
        orderedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var ordered = await ReadArrayAsync(orderedResponse);
        ordered[0].GetProperty("id").GetInt64().Should().Be(secondId);
        ordered[0].GetProperty("sortOrder").GetInt32().Should().Be(0);
        ordered[1].GetProperty("id").GetInt64().Should().Be(firstId);
        ordered[1].GetProperty("sortOrder").GetInt32().Should().Be(1);

        // Reorder mutates the entity and therefore rotates its concurrency token.
        var firstAfterReorder = ordered.Single(x => x.GetProperty("id").GetInt64() == firstId);
        var tokenAfterReorder = firstAfterReorder.GetProperty("concurrencyToken").GetGuid();

        var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{firstId}")
        {
            Content = JsonContent.Create(new { concurrencyToken = tokenAfterReorder })
        };

        var delete = await client.SendAsync(deleteRequest);
        delete.IsSuccessStatusCode.Should().BeTrue();

        var withDeletedResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters?includeDeleted=true");
        withDeletedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var withDeleted = await ReadArrayAsync(withDeletedResponse);
        var deleted = withDeleted.Single(x => x.GetProperty("id").GetInt64() == firstId);
        deleted.GetProperty("deletedAt").ValueKind.Should().NotBe(JsonValueKind.Null);
        var deletedToken = deleted.GetProperty("concurrencyToken").GetGuid();

        var restore = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{firstId}/restore",
            new { concurrencyToken = deletedToken });

        restore.StatusCode.Should().Be(HttpStatusCode.OK);
        var restored = await ReadObjectAsync(restore);
        restored.GetProperty("deletedAt").ValueKind.Should().Be(JsonValueKind.Null);
        restored.GetProperty("isActive").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task Admin_CanAssignMoveReorderAndRemoveLessonsBetweenChapters()
    {
        var adminId = await CreateUserAsync("course-lessons-admin");
        var seed = await CreateDraftCourseWithLessonsAsync();
        var client = Factory.CreateAdminClient(adminId);

        var chapterOne = await CreateChapterAsync(client, seed.CourseId, "Chương A", 0);
        var chapterTwo = await CreateChapterAsync(client, seed.CourseId, "Chương B", 1);

        var assignFirst = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons/assign",
            new { lessonId = seed.LessonOneId, sortOrder = 0 });
        assignFirst.StatusCode.Should().Be(HttpStatusCode.OK);

        var assignSecond = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons/assign",
            new { lessonId = seed.LessonTwoId, sortOrder = 1 });
        assignSecond.StatusCode.Should().Be(HttpStatusCode.OK);

        var listOne = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons");
        listOne.StatusCode.Should().Be(HttpStatusCode.OK);
        var lessons = await ReadArrayAsync(listOne);
        lessons.Select(x => x.GetProperty("id").GetInt64())
            .Should().Contain(new[] { seed.LessonOneId, seed.LessonTwoId });

        var reorder = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons/reorder",
            new
            {
                items = new[]
                {
                    new { lessonId = seed.LessonOneId, sortOrder = 1 },
                    new { lessonId = seed.LessonTwoId, sortOrder = 0 }
                }
            });
        reorder.IsSuccessStatusCode.Should().BeTrue();

        var reorderedResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons");
        reorderedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reordered = await ReadArrayAsync(reorderedResponse);
        reordered[0].GetProperty("id").GetInt64().Should().Be(seed.LessonTwoId);
        reordered[0].GetProperty("sortOrder").GetInt32().Should().Be(0);

        var move = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons/{seed.LessonTwoId}/move",
            new { targetChapterId = chapterTwo, sortOrder = 0 });
        move.StatusCode.Should().Be(HttpStatusCode.OK);

        var sourceResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterOne}/lessons");
        sourceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var sourceLessons = await ReadArrayAsync(sourceResponse);
        sourceLessons.Should().NotContain(x => x.GetProperty("id").GetInt64() == seed.LessonTwoId);

        var targetResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterTwo}/lessons");
        targetResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var targetLessons = await ReadArrayAsync(targetResponse);
        targetLessons.Should().ContainSingle(x => x.GetProperty("id").GetInt64() == seed.LessonTwoId);

        var remove = await client.DeleteAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterTwo}/lessons/{seed.LessonTwoId}");
        remove.IsSuccessStatusCode.Should().BeTrue();

        var afterRemoveResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters/{chapterTwo}/lessons");
        afterRemoveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var afterRemove = await ReadArrayAsync(afterRemoveResponse);
        afterRemove.Should().BeEmpty();
    }

    [Fact]
    public async Task AnonymousUser_CannotManageCourseChapters()
    {
        var seed = await CreateDraftCourseWithLessonsAsync();

        var response = await Factory.CreateAnonymousClient().GetAsync(
            $"/api/v1/admin/courses/{seed.CourseId}/chapters?includeDeleted=false");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<CourseSeed> CreateDraftCourseWithLessonsAsync()
    {
        return await Factory.ExecuteDbAsync(async db =>
        {
            var hskId = await db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync();

            var suffix = Guid.NewGuid().ToString("N")[..8];
            var course = new Domain.Entities.Course.Course(
                $"COURSE-{suffix}",
                $"course-{suffix}",
                $"Khóa học {suffix}",
                hskId);

            db.Add(course);
            await db.SaveChangesAsync();

            var lessonOne = new Domain.Entities.Lesson.Lesson(
                hskId,
                $"lesson-one-{suffix}",
                "Bài giảng 1");
            var lessonTwo = new Domain.Entities.Lesson.Lesson(
                hskId,
                $"lesson-two-{suffix}",
                "Bài giảng 2");

            db.AddRange(lessonOne, lessonTwo);
            await db.SaveChangesAsync();

            return new CourseSeed(course.Id, lessonOne.Id, lessonTwo.Id);
        });
    }

    private static async Task<long> CreateChapterAsync(
        HttpClient client,
        long courseId,
        string title,
        int sortOrder)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{courseId}/chapters",
            new
            {
                titleVi = title,
                descriptionVi = (string?)null,
                sortOrder,
                isActive = true
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadObjectAsync(response);
        return result.GetProperty("id").GetInt64();
    }

    private static async Task<JsonElement> ReadObjectAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    private static async Task<JsonElement[]> ReadArrayAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.EnumerateArray().Select(x => x.Clone()).ToArray();
    }

    private sealed record CourseSeed(long CourseId, long LessonOneId, long LessonTwoId);
}
