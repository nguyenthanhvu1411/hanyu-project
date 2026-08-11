using System.Net;
using System.Text.Json;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Course;

using Common;

public sealed class CourseInsightsIntegrationTests : IntegrationTestBase
{
    public CourseInsightsIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CourseAndLesson_SlugsAreGenerated_WhenAdminOmitsThem()
    {
        var adminId = await CreateUserAsync("course-slug-admin");
        var client = Factory.CreateAdminClient(adminId);
        var hskId = await GetHsk1IdAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var createCourse = await client.PostAsJsonAsync(
            "/api/v1/admin/courses",
            new
            {
                code = $"SLUG-{suffix}",
                slug = "",
                titleVi = $"Khóa học Tiếng Trung Cơ Bản {suffix}",
                hskLevelId = hskId,
                sortOrder = 0,
                estimatedMinutes = 60,
                isFeatured = false
            });

        createCourse.StatusCode.Should().Be(HttpStatusCode.OK);
        var course = await ReadObjectAsync(createCourse);
        var courseId = course.GetProperty("id").GetInt64();
        course.GetProperty("slug").GetString()
            .Should().Be($"khoa-hoc-tieng-trung-co-ban-{suffix}");

        var createChapter = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{courseId}/chapters",
            new
            {
                titleVi = "Nhập môn",
                descriptionVi = (string?)null,
                sortOrder = 0,
                isActive = true
            });

        createChapter.StatusCode.Should().Be(HttpStatusCode.OK);
        var chapter = await ReadObjectAsync(createChapter);
        var chapterId = chapter.GetProperty("id").GetInt64();

        var createLesson = await client.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = chapterId,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = "",
                titleVi = $"Chào Hỏi Và Giới Thiệu Bản Thân {suffix}",
                shortDescriptionVi = "Kiểm tra slug backend",
                descriptionVi = (string?)null,
                objectiveVi = (string?)null,
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });

        createLesson.StatusCode.Should().Be(HttpStatusCode.OK);
        var lesson = await ReadObjectAsync(createLesson);
        lesson.GetProperty("slug").GetString()
            .Should().Be($"chao-hoi-va-gioi-thieu-ban-than-{suffix}");
    }

    [Fact]
    public async Task History_ReturnsPersistedAudit_WhenCourseIsUpdated()
    {
        var adminId = await CreateUserAsync("course-history-admin");
        var client = Factory.CreateAdminClient(adminId);
        var course = await CreateCourseAsync(client, "Lịch sử thật");

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{course.Id}",
            new
            {
                code = course.Code,
                slug = course.Slug,
                titleVi = "Lịch sử thật - đã cập nhật",
                shortDescriptionVi = "Thay đổi để tạo audit",
                descriptionVi = (string?)null,
                hskLevelId = course.HskLevelId,
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 90,
                isFeatured = false,
                concurrencyToken = course.ConcurrencyToken
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);

        var historyResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{course.Id}/history");

        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await ReadArrayAsync(historyResponse);

        history.Should().Contain(item =>
            item.GetProperty("action").GetString() == "updated" &&
            item.TryGetProperty("changedPropertiesJson", out var changed) &&
            changed.ValueKind == JsonValueKind.String &&
            changed.GetString()!.Contains("TitleVi", StringComparison.Ordinal));
    }

    [Fact]
    public async Task StatisticsAndStudents_AreCalculatedFromRealLessonProgress()
    {
        var adminId = await CreateUserAsync("course-insights-admin");
        var studentId = await CreateUserAsync("course-student");
        var client = Factory.CreateAdminClient(adminId);
        var hskId = await GetHsk1IdAsync();
        var course = await CreateCourseAsync(client, "Khóa học có học viên");

        var chapterResponse = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{course.Id}/chapters",
            new
            {
                titleVi = "Chương tiến độ",
                descriptionVi = (string?)null,
                sortOrder = 0,
                isActive = true
            });
        chapterResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var chapter = await ReadObjectAsync(chapterResponse);
        var chapterId = chapter.GetProperty("id").GetInt64();

        var lessonResponse = await client.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = chapterId,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = "",
                titleVi = "Bài tiến độ thật",
                shortDescriptionVi = (string?)null,
                descriptionVi = (string?)null,
                objectiveVi = (string?)null,
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });
        lessonResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var lesson = await ReadObjectAsync(lessonResponse);
        var lessonId = lesson.GetProperty("id").GetInt64();

        await Factory.ExecuteDbAsync(async db =>
        {
            var progress = new UserLessonProgress(studentId, lessonId);
            progress.Start();
            progress.UpdateProgress(null, 0, 50m);
            db.Add(progress);
            await db.SaveChangesAsync();
        });

        var statisticsResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{course.Id}/statistics");
        statisticsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await ReadObjectAsync(statisticsResponse);

        statistics.GetProperty("totalChapters").GetInt32().Should().Be(1);
        statistics.GetProperty("activeChapters").GetInt32().Should().Be(1);
        statistics.GetProperty("totalLessons").GetInt32().Should().Be(1);
        statistics.GetProperty("totalStudents").GetInt32().Should().Be(1);
        statistics.GetProperty("studentsInProgress").GetInt32().Should().Be(1);
        statistics.GetProperty("studentsCompleted").GetInt32().Should().Be(0);
        statistics.GetProperty("averageCompletionPercent").GetDecimal().Should().Be(50m);

        var studentsResponse = await client.GetAsync(
            $"/api/v1/admin/courses/{course.Id}/students?page=1&pageSize=20&status=in_progress");
        studentsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var students = await ReadObjectAsync(studentsResponse);

        students.GetProperty("total").GetInt32().Should().Be(1);
        var student = students.GetProperty("items").EnumerateArray().Single();
        student.GetProperty("userId").GetGuid().Should().Be(studentId);
        student.GetProperty("startedLessons").GetInt32().Should().Be(1);
        student.GetProperty("completedLessons").GetInt32().Should().Be(0);
        student.GetProperty("totalLessons").GetInt32().Should().Be(1);
        student.GetProperty("completionPercent").GetDecimal().Should().Be(50m);
        student.GetProperty("status").GetString().Should().Be("in_progress");
    }

    [Fact]
    public async Task AnonymousUser_CannotReadCourseInsights()
    {
        var adminId = await CreateUserAsync("course-insights-owner");
        var admin = Factory.CreateAdminClient(adminId);
        var course = await CreateCourseAsync(admin, "Khóa học bảo mật");
        var anonymous = Factory.CreateAnonymousClient();

        (await anonymous.GetAsync($"/api/v1/admin/courses/{course.Id}/history"))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await anonymous.GetAsync($"/api/v1/admin/courses/{course.Id}/statistics"))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await anonymous.GetAsync($"/api/v1/admin/courses/{course.Id}/students"))
            .StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<long> GetHsk1IdAsync()
        => await Factory.ExecuteDbAsync(db =>
            db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync());

    private async Task<CourseSeed> CreateCourseAsync(HttpClient client, string title)
    {
        var hskId = await GetHsk1IdAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/courses",
            new
            {
                code = $"INSIGHT-{suffix}",
                slug = "",
                titleVi = $"{title} {suffix}",
                shortDescriptionVi = (string?)null,
                descriptionVi = (string?)null,
                hskLevelId = hskId,
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 90,
                isFeatured = false
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var course = await ReadObjectAsync(response);
        return new CourseSeed(
            course.GetProperty("id").GetInt64(),
            course.GetProperty("code").GetString()!,
            course.GetProperty("slug").GetString()!,
            course.GetProperty("hskLevelId").GetInt64(),
            course.GetProperty("concurrencyToken").GetGuid());
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

    private sealed record CourseSeed(
        long Id,
        string Code,
        string Slug,
        long HskLevelId,
        Guid ConcurrencyToken);
}
