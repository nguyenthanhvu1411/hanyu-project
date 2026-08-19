using System.Net;
using System.Text.Json;
using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;
using CourseEntity = HanYu.Domain.Entities.Course.Course;
using CourseChapterEntity = HanYu.Domain.Entities.Course.CourseChapter;

namespace HanYu.IntegrationTests.Lesson;

using Common;

public sealed class LessonPublicWorkflowIntegrationTests
    : IntegrationTestBase
{
    public LessonPublicWorkflowIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Draft_ToPublished_ToArchived_UpdatesPublicLessonAndCourseCaches()
    {
        var adminId = await CreateUserAsync("lesson-workflow-admin");
        var admin = Factory.CreateAdminClient(adminId);
        var anonymous = Factory.CreateAnonymousClient();
        var course = await CreateDraftCourseWithChapterAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var lessonSlug = $"lesson-workflow-{suffix}";

        var createLesson = await admin.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = course.ChapterId,
                hskLevelId = course.HskLevelId,
                topicId = (long?)null,
                slug = lessonSlug,
                titleVi = $"Lesson workflow {suffix}",
                shortDescriptionVi = "Integration test workflow",
                descriptionVi = (string?)null,
                objectiveVi = "Hoàn thành bài học thử nghiệm",
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });

        await AssertSuccessAsync(createLesson);
        var lesson = await ReadObjectAsync(createLesson);
        var lessonId = lesson.GetProperty("id").GetInt64();
        var lessonPublicId = lesson.GetProperty("publicId").GetGuid();
        var version = lesson.GetProperty("version").GetInt32();

        var createSection = await admin.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lessonId}/sections",
            new
            {
                sectionType = 2,
                sortOrder = 0,
                titleVi = "Nội dung chính",
                contentVi = "Nội dung hợp lệ để Lesson có thể gửi duyệt và xuất bản.",
                isRequired = true,
                estimatedSeconds = 180
            });

        await AssertSuccessAsync(createSection);
        await PublishCourseAsync(course.CourseId, adminId);

        // Warm both public caches while Lesson is still Draft.
        var warmCourseLessons = await anonymous.GetAsync(
            $"/api/v1/public/courses/{course.CourseSlug}/lessons");
        await AssertSuccessAsync(warmCourseLessons);
        (await warmCourseLessons.Content.ReadAsStringAsync())
            .Should().NotContain(lessonPublicId.ToString());

        var warmLessonList = await anonymous.GetAsync(
            "/api/v1/public/lessons?page=1&pageSize=100");
        await AssertSuccessAsync(warmLessonList);
        (await warmLessonList.Content.ReadAsStringAsync())
            .Should().NotContain(lessonPublicId.ToString());

        var submit = await admin.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lessonId}/submit-review",
            new { version });
        await AssertSuccessAsync(submit);
        version = (await ReadObjectAsync(submit)).GetProperty("version").GetInt32();

        var approve = await admin.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lessonId}/approve",
            new { version });
        await AssertSuccessAsync(approve);
        version = (await ReadObjectAsync(approve)).GetProperty("version").GetInt32();

        var publish = await admin.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lessonId}/publish",
            new { version });
        await AssertSuccessAsync(publish);
        version = (await ReadObjectAsync(publish)).GetProperty("version").GetInt32();

        var publicBySlug = await anonymous.GetAsync(
            $"/api/v1/public/lessons/{lessonSlug}");
        publicBySlug.StatusCode.Should().Be(HttpStatusCode.OK);

        var courseLessonsAfterPublish = await anonymous.GetAsync(
            $"/api/v1/public/courses/{course.CourseSlug}/lessons");
        await AssertSuccessAsync(courseLessonsAfterPublish);
        (await courseLessonsAfterPublish.Content.ReadAsStringAsync())
            .Should().Contain(lessonPublicId.ToString());

        var lessonListAfterPublish = await anonymous.GetAsync(
            "/api/v1/public/lessons?page=1&pageSize=100");
        await AssertSuccessAsync(lessonListAfterPublish);
        (await lessonListAfterPublish.Content.ReadAsStringAsync())
            .Should().Contain(lessonPublicId.ToString());

        var archive = await admin.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{lessonId}/archive",
            new { version });
        await AssertSuccessAsync(archive);

        var publicAfterArchive = await anonymous.GetAsync(
            $"/api/v1/public/lessons/{lessonSlug}");
        publicAfterArchive.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var courseLessonsAfterArchive = await anonymous.GetAsync(
            $"/api/v1/public/courses/{course.CourseSlug}/lessons");
        await AssertSuccessAsync(courseLessonsAfterArchive);
        (await courseLessonsAfterArchive.Content.ReadAsStringAsync())
            .Should().NotContain(lessonPublicId.ToString());

        var lessonListAfterArchive = await anonymous.GetAsync(
            "/api/v1/public/lessons?page=1&pageSize=100");
        await AssertSuccessAsync(lessonListAfterArchive);
        (await lessonListAfterArchive.Content.ReadAsStringAsync())
            .Should().NotContain(lessonPublicId.ToString());
    }

    [Fact]
    public async Task CourseLessons_ReturnsOnlyPublishedLessons_FromActivePublishedCourse()
    {
        var adminId = await CreateUserAsync("course-lessons-admin");
        var admin = Factory.CreateAdminClient(adminId);
        var anonymous = Factory.CreateAnonymousClient();
        var course = await CreateDraftCourseWithChapterAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var draftResponse = await admin.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = course.ChapterId,
                hskLevelId = course.HskLevelId,
                topicId = (long?)null,
                slug = $"course-draft-{suffix}",
                titleVi = $"Course draft {suffix}",
                shortDescriptionVi = (string?)null,
                descriptionVi = (string?)null,
                objectiveVi = (string?)null,
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });
        await AssertSuccessAsync(draftResponse);
        var draftPublicId = (await ReadObjectAsync(draftResponse))
            .GetProperty("publicId")
            .GetGuid();

        await PublishCourseAsync(course.CourseId, adminId);

        var response = await anonymous.GetAsync(
            $"/api/v1/public/courses/{course.CourseSlug}/lessons");

        await AssertSuccessAsync(response);
        (await response.Content.ReadAsStringAsync())
            .Should().NotContain(draftPublicId.ToString());
    }

    private async Task<CourseSeed> CreateDraftCourseWithChapterAsync()
    {
        return await Factory.ExecuteDbAsync(async db =>
        {
            var hskLevelId = await db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync();

            var suffix = Guid.NewGuid().ToString("N")[..8];
            var course = new CourseEntity(
                $"WF-{suffix}",
                $"workflow-course-{suffix}",
                $"Workflow Course {suffix}",
                hskLevelId,
                estimatedMinutes: 60);

            db.Add(course);
            await db.SaveChangesAsync();

            var chapter = new CourseChapterEntity(
                course.Id,
                "Chương workflow",
                sortOrder: 0,
                isActive: true);

            db.Add(chapter);
            await db.SaveChangesAsync();

            return new CourseSeed(
                course.Id,
                course.Slug,
                chapter.Id,
                hskLevelId);
        });
    }

    private async Task PublishCourseAsync(
        long courseId,
        Guid adminId)
    {
        await Factory.ExecuteDbAsync(async db =>
        {
            var course = await db.Set<CourseEntity>()
                .Include(x => x.Chapters)
                .SingleAsync(x => x.Id == courseId);

            course.SubmitForReview(adminId);
            course.Approve(adminId);
            course.Publish(adminId);

            await db.SaveChangesAsync();
        });
    }

    private static async Task<JsonElement> ReadObjectAsync(
        HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        return document.RootElement.Clone();
    }

    private static async Task AssertSuccessAsync(
        HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        response.IsSuccessStatusCode
            .Should()
            .BeTrue(
                $"HTTP {(int)response.StatusCode} {response.StatusCode}; body: {body}");
    }

    private sealed record CourseSeed(
        long CourseId,
        string CourseSlug,
        long ChapterId,
        long HskLevelId);
}
