using System.Net;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Lesson;

using Common;

public sealed class LessonAssetUsageIntegrationTests : IntegrationTestBase
{
    public LessonAssetUsageIntegrationTests(HanYuWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task AssetMustBeDetachedFromSectionsBeforeRemoval()
    {
        var adminId = await CreateUserAsync("admin-asset-usage");
        var fixture = await Factory.ExecuteDbAsync(async db =>
        {
            var hskId = await db.Set<HanYu.Domain.Entities.Vocabulary.HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync();
            var lesson = new Domain.Entities.Lesson.Lesson(
                hskId,
                Unique("asset-usage"),
                "Asset usage integration");
            db.Add(lesson);
            await db.SaveChangesAsync();

            var section = new LessonSection(
                lesson.Id,
                LessonSectionType.Explanation,
                0,
                "Section có media");
            var asset = new LessonAsset(
                lesson.Id,
                LessonAssetType.Image,
                0);
            asset.Update("/media/asset-usage.png", "Ảnh sử dụng", null, 0);
            db.Add(section);
            db.Add(asset);
            await db.SaveChangesAsync();

            var link = new LessonSectionAsset(section.Id, asset.Id, 0, "Ảnh section", true);
            db.Add(link);
            await db.SaveChangesAsync();
            return (lesson.Id, SectionId: section.Id, AssetId: asset.Id, LinkId: link.Id);
        });

        var client = Factory.CreateAdminClient(adminId);
        var firstAttempt = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{fixture.Id}/assets/{fixture.AssetId}");
        firstAttempt.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await firstAttempt.Content.ReadAsStringAsync()).Should().Contain("LessonAsset.InUse");

        var state = await Factory.ExecuteDbAsync(async db =>
        {
            var assetExists = await db.Set<LessonAsset>().AnyAsync(x => x.Id == fixture.AssetId);
            var linkExists = await db.Set<LessonSectionAsset>().AnyAsync(x => x.Id == fixture.LinkId);
            return (assetExists, linkExists);
        });
        state.assetExists.Should().BeTrue();
        state.linkExists.Should().BeTrue();

        var detach = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{fixture.Id}/sections/{fixture.SectionId}/assets/{fixture.LinkId}");
        detach.IsSuccessStatusCode.Should().BeTrue();

        var secondAttempt = await client.DeleteAsync(
            $"/api/v1/admin/lessons/{fixture.Id}/assets/{fixture.AssetId}");
        secondAttempt.IsSuccessStatusCode.Should().BeTrue();

        var assetStillExists = await Factory.ExecuteDbAsync(async db =>
            await db.Set<LessonAsset>().AnyAsync(x => x.Id == fixture.AssetId));
        assetStillExists.Should().BeFalse();
    }
}
