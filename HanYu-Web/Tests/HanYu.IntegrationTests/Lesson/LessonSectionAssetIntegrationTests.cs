using System.Text.Json;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Lesson;

using Common;

public sealed class LessonSectionAssetIntegrationTests : IntegrationTestBase
{
    public LessonSectionAssetIntegrationTests(HanYuWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Admin_CanAttachUpdateReorderAndDetachSectionMedia()
    {
        var adminId = await CreateUserAsync("admin-section-media");
        var fixture = await CreateFixtureAsync();
        var client = Factory.CreateAdminClient(adminId);
        var root = $"/api/v1/admin/lessons/{fixture.LessonId}/sections/{fixture.SectionId}/assets";

        var first = await client.PostAsJsonAsync(root, new
        {
            lessonAssetId = fixture.ImageAssetId,
            sortOrder = 0,
            captionVi = "Ảnh section",
            isRequired = true
        });
        await AssertSuccessAsync(first);
        var firstId = await ReadIdAsync(first);

        var second = await client.PostAsJsonAsync(root, new
        {
            lessonAssetId = fixture.DocumentAssetId,
            sortOrder = 1,
            captionVi = "Tài liệu",
            isRequired = false
        });
        await AssertSuccessAsync(second);
        var secondId = await ReadIdAsync(second);

        await AssertSuccessAsync(await client.PutAsJsonAsync($"{root}/{secondId}", new
        {
            sortOrder = 0,
            captionVi = "Tài liệu cập nhật",
            isRequired = true
        }));
        await AssertSuccessAsync(await client.PutAsJsonAsync($"{root}/{firstId}", new
        {
            sortOrder = 1,
            captionVi = "Ảnh sau reorder",
            isRequired = false
        }));

        var list = await client.GetAsync(root);
        await AssertSuccessAsync(list);
        using (var json = JsonDocument.Parse(await list.Content.ReadAsStringAsync()))
        {
            var items = json.RootElement.EnumerateArray().ToArray();
            items.Should().HaveCount(2);
            items[0].GetProperty("id").GetInt64().Should().Be(secondId);
            items[0].GetProperty("sortOrder").GetInt32().Should().Be(0);
            items[0].GetProperty("captionVi").GetString().Should().Be("Tài liệu cập nhật");
            items[0].GetProperty("isRequired").GetBoolean().Should().BeTrue();
            items[1].GetProperty("id").GetInt64().Should().Be(firstId);
            items[1].GetProperty("sortOrder").GetInt32().Should().Be(1);
        }

        await AssertSuccessAsync(await client.DeleteAsync($"{root}/{firstId}"));
        var after = await client.GetAsync(root);
        await AssertSuccessAsync(after);
        (await after.Content.ReadAsStringAsync()).Should().NotContain("Ảnh sau reorder");
    }

    [Fact]
    public async Task PublicLesson_ReturnsSectionMediaInConfiguredOrder()
    {
        var data = await TestDataSeeder.SeedLearningDataAsync(Factory);
        var ids = await Factory.ExecuteDbAsync(async db =>
        {
            var section = await db.Set<LessonSection>()
                .SingleAsync(x => x.PublicId == data.SectionPublicId);

            var image = new LessonAsset(data.LessonId, LessonAssetType.Image, 0);
            image.Update("/media/public-image.png", "Ảnh gốc", null, 0);
            var document = new LessonAsset(data.LessonId, LessonAssetType.Document, 1);
            document.Update("/media/public-document.pdf", "Tài liệu gốc", null, 1);
            db.Add(image);
            db.Add(document);
            await db.SaveChangesAsync();

            db.Add(new LessonSectionAsset(section.Id, image.Id, 1, "Ảnh public", false));
            db.Add(new LessonSectionAsset(section.Id, document.Id, 0, "Tài liệu public", true));
            await db.SaveChangesAsync();

            return (image.PublicId, document.PublicId);
        });

        var response = await Factory.CreateAnonymousClient()
            .GetAsync($"/api/v1/public/lessons/{data.LessonPublicId}");
        await AssertSuccessAsync(response);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var sectionJson = json.RootElement.GetProperty("sections").EnumerateArray()
            .Single(x => x.GetProperty("publicId").GetGuid() == data.SectionPublicId);
        var media = sectionJson.GetProperty("media").EnumerateArray().ToArray();

        media.Should().HaveCount(2);
        media[0].GetProperty("assetPublicId").GetGuid().Should().Be(ids.Item2);
        media[0].GetProperty("sortOrder").GetInt32().Should().Be(0);
        media[0].GetProperty("captionVi").GetString().Should().Be("Tài liệu public");
        media[0].GetProperty("isRequired").GetBoolean().Should().BeTrue();
        media[1].GetProperty("assetPublicId").GetGuid().Should().Be(ids.Item1);
        media[1].GetProperty("sortOrder").GetInt32().Should().Be(1);
        media[1].GetProperty("captionVi").GetString().Should().Be("Ảnh public");
    }

    private async Task<Fixture> CreateFixtureAsync()
    {
        return await Factory.ExecuteDbAsync(async db =>
        {
            var hskId = await db.Set<HanYu.Domain.Entities.Vocabulary.HskLevel>()
                .Where(x => x.Code == "HSK1").Select(x => x.Id).SingleAsync();
            var lesson = new Domain.Entities.Lesson.Lesson(hskId, Unique("section-media"), "Lesson Section Media Integration");
            db.Add(lesson);
            await db.SaveChangesAsync();

            var section = new LessonSection(lesson.Id, LessonSectionType.Explanation, 0, "Section media");
            var image = new LessonAsset(lesson.Id, LessonAssetType.Image, 0);
            image.Update("/media/section-image.png", "Ảnh lesson", null, 0);
            var document = new LessonAsset(lesson.Id, LessonAssetType.Document, 1);
            document.Update("/media/section-document.pdf", "Tài liệu lesson", null, 1);
            db.Add(section);
            db.Add(image);
            db.Add(document);
            await db.SaveChangesAsync();
            return new Fixture(lesson.Id, section.Id, image.Id, document.Id);
        });
    }

    private static async Task<long> ReadIdAsync(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("id").GetInt64();
    }

    private static async Task AssertSuccessAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue($"HTTP {(int)response.StatusCode} {response.StatusCode}; body: {body}");
    }

    private sealed record Fixture(long LessonId, long SectionId, long ImageAssetId, long DocumentAssetId);
}
