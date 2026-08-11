using System.Net;
using System.Text.Json;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Lesson;

public sealed class LessonCoverStorageIntegrationTests : IntegrationTestBase
{
    public LessonCoverStorageIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Lesson_CoverStorageReference_SurvivesCreateReadAndUpdate()
    {
        var adminId = await CreateUserAsync("lesson-cover-admin");
        var client = Factory.CreateAdminClient(adminId);
        var hskId = await Factory.ExecuteDbAsync(async db =>
            await db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync());

        var suffix = Guid.NewGuid().ToString("N")[..8];
        var firstCover = $"storage://images/lesson-covers/{suffix}-01.png";
        var secondCover = $"storage://images/lesson-covers/{suffix}-02.webp";

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = (long?)null,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = $"lesson-cover-{suffix}",
                titleVi = $"Bài giảng ảnh {suffix}",
                shortDescriptionVi = "Kiểm tra ảnh bìa Storage",
                descriptionVi = "Nội dung kiểm tra",
                objectiveVi = "Ảnh bìa vẫn tồn tại sau khi tải lại",
                coverImageUrl = firstCover,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await ReadObjectAsync(create);
        var id = created.GetProperty("id").GetInt64();
        var version = created.GetProperty("version").GetInt32();
        created.GetProperty("coverImageUrl").GetString().Should().Be(firstCover);

        var detailResponse = await client.GetAsync($"/api/v1/admin/lessons/{id}");
        detailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await ReadObjectAsync(detailResponse);
        detail.GetProperty("coverImageUrl").GetString().Should().Be(firstCover);

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/lessons/{id}",
            new
            {
                courseChapterId = (long?)null,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = $"lesson-cover-{suffix}",
                titleVi = $"Bài giảng ảnh {suffix}",
                shortDescriptionVi = "Kiểm tra ảnh bìa Storage",
                descriptionVi = "Nội dung kiểm tra",
                objectiveVi = "Ảnh bìa vẫn tồn tại sau khi tải lại",
                coverImageUrl = secondCover,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false,
                version
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(update)).GetProperty("coverImageUrl").GetString().Should().Be(secondCover);

        var reloadedResponse = await client.GetAsync($"/api/v1/admin/lessons/{id}");
        reloadedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(reloadedResponse)).GetProperty("coverImageUrl").GetString().Should().Be(secondCover);
    }

    private static async Task<JsonElement> ReadObjectAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
