using System.Net;
using HanYu.Domain.Entities.Vocabulary;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Content;

using Common;

public sealed class SlugAvailabilityIntegrationTests : IntegrationTestBase
{
    public SlugAvailabilityIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CourseSlugAvailability_ReturnsFalseForExistingSlug()
    {
        var adminId = await CreateUserAsync("slug-course-existing");
        var client = Factory.CreateAdminClient(adminId);
        var seed = await SeedSlugEntitiesAsync();

        var response = await client.GetAsync(
            $"/api/v1/admin/courses/slug-availability?slug={seed.CourseSlug}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SlugAvailabilityDto>();
        body.Should().NotBeNull();
        body!.Available.Should().BeFalse();
        body.Slug.Should().Be(seed.CourseSlug);
    }

    [Fact]
    public async Task LessonSlugAvailability_ReturnsTrueForUnusedSlug()
    {
        var adminId = await CreateUserAsync("slug-lesson-unused");
        var client = Factory.CreateAdminClient(adminId);
        await SeedSlugEntitiesAsync();
        var slug = Unique("lesson-available");

        var response = await client.GetAsync(
            $"/api/v1/admin/lessons/slug-availability?slug={slug}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SlugAvailabilityDto>();
        body.Should().NotBeNull();
        body!.Available.Should().BeTrue();
        body.Slug.Should().Be(slug);
    }

    [Fact]
    public async Task TopicSlugAvailability_ExcludeId_AllowsCurrentTopicSlug()
    {
        var adminId = await CreateUserAsync("slug-topic-exclude");
        var client = Factory.CreateAdminClient(adminId);
        var seed = await SeedSlugEntitiesAsync();

        var response = await client.GetAsync(
            $"/api/v1/admin/vocabulary-topics/slug-availability?slug={seed.TopicSlug}&excludeId={seed.TopicId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<SlugAvailabilityDto>();
        body.Should().NotBeNull();
        body!.Available.Should().BeTrue();
    }

    [Fact]
    public async Task CourseCreate_DuplicateSlug_ReturnsConflictWithStableCode()
    {
        var adminId = await CreateUserAsync("slug-course-create");
        var client = Factory.CreateAdminClient(adminId);
        var seed = await SeedSlugEntitiesAsync();

        var response = await client.PostAsJsonAsync(
            "/api/v1/admin/courses",
            new
            {
                code = Unique("COURSE").ToUpperInvariant(),
                slug = seed.CourseSlug,
                titleVi = "Khóa học trùng slug",
                hskLevelId = (long?)null,
                sortOrder = 0,
                estimatedMinutes = (int?)null,
                isFeatured = false
            });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync())
            .Should().Contain("Course.SlugAlreadyExists");
    }

    [Fact]
    public async Task CourseUpdate_ToAnotherCourseSlug_ReturnsConflict()
    {
        var adminId = await CreateUserAsync("slug-course-update-conflict");
        var client = Factory.CreateAdminClient(adminId);
        var seed = await SeedTwoCoursesAsync();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{seed.SecondId}",
            new
            {
                code = seed.SecondCode,
                slug = seed.FirstSlug,
                titleVi = "Khóa học thứ hai",
                hskLevelId = (long?)null,
                shortDescriptionVi = (string?)null,
                descriptionVi = (string?)null,
                coverImageUrl = (string?)null,
                sortOrder = 1,
                estimatedMinutes = (int?)null,
                isFeatured = false,
                concurrencyToken = seed.SecondConcurrencyToken
            });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync())
            .Should().Contain("Course.SlugAlreadyExists");
    }

    [Fact]
    public async Task CourseUpdate_KeepingCurrentSlug_Succeeds()
    {
        var adminId = await CreateUserAsync("slug-course-update-same");
        var client = Factory.CreateAdminClient(adminId);
        var seed = await SeedTwoCoursesAsync();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{seed.SecondId}",
            new
            {
                code = seed.SecondCode,
                slug = seed.SecondSlug,
                titleVi = "Khóa học thứ hai đã cập nhật",
                hskLevelId = (long?)null,
                shortDescriptionVi = (string?)null,
                descriptionVi = (string?)null,
                coverImageUrl = (string?)null,
                sortOrder = 1,
                estimatedMinutes = (int?)null,
                isFeatured = false,
                concurrencyToken = seed.SecondConcurrencyToken
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<SlugSeed> SeedSlugEntitiesAsync()
    {
        return await Factory.ExecuteDbAsync(async db =>
        {
            var hskId = await db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync();

            var suffix = Guid.NewGuid().ToString("N")[..8];
            var courseSlug = $"course-slug-{suffix}";
            var lessonSlug = $"lesson-slug-{suffix}";
            var topicSlug = $"topic-slug-{suffix}";

            var course = new Domain.Entities.Course.Course(
                $"COURSE-{suffix}",
                courseSlug,
                "Khóa học test slug",
                null,
                0);

            var lesson = new Domain.Entities.Lesson.Lesson(
                hskId,
                lessonSlug,
                "Bài giảng test slug",
                0);

            var topic = new Topic(
                topicSlug,
                "Chủ đề test slug",
                null,
                0);

            db.AddRange(course, lesson, topic);
            await db.SaveChangesAsync();

            return new SlugSeed(
                course.Id,
                courseSlug,
                lesson.Id,
                lessonSlug,
                topic.Id,
                topicSlug);
        });
    }

    private async Task<TwoCourseSeed> SeedTwoCoursesAsync()
    {
        return await Factory.ExecuteDbAsync(async db =>
        {
            var suffix = Guid.NewGuid().ToString("N")[..8];
            var first = new Domain.Entities.Course.Course(
                $"FIRST-{suffix}",
                $"first-{suffix}",
                "Khóa học thứ nhất",
                null,
                0);
            var second = new Domain.Entities.Course.Course(
                $"SECOND-{suffix}",
                $"second-{suffix}",
                "Khóa học thứ hai",
                null,
                1);

            db.AddRange(first, second);
            await db.SaveChangesAsync();

            return new TwoCourseSeed(
                first.Id,
                first.Slug,
                second.Id,
                second.Code,
                second.Slug,
                second.ConcurrencyToken);
        });
    }

    private sealed record SlugAvailabilityDto(string Slug, bool Available);

    private sealed record SlugSeed(
        long CourseId,
        string CourseSlug,
        long LessonId,
        string LessonSlug,
        long TopicId,
        string TopicSlug);

    private sealed record TwoCourseSeed(
        long FirstId,
        string FirstSlug,
        long SecondId,
        string SecondCode,
        string SecondSlug,
        Guid SecondConcurrencyToken);
}
