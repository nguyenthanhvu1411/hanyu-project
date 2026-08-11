using System.Net;
using System.Text.Json;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Content;

public sealed class AdminLearningContentCrudIntegrationTests : IntegrationTestBase
{
    public AdminLearningContentCrudIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task HskLevel_CreateReadUpdateStateDeleteAndRestore_Works()
    {
        var adminId = await CreateUserAsync("hsk-crud-admin");
        var client = Factory.CreateAdminClient(adminId);
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/hsk-levels",
            new
            {
                code = $"HX{suffix}",
                nameVi = $"HSK test {suffix}",
                sortOrder = 99
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await ReadObjectAsync(create);
        var id = created.GetProperty("id").GetInt64();

        var detail = await client.GetAsync($"/api/v1/admin/hsk-levels/{id}");
        detail.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/hsk-levels/{id}",
            new
            {
                code = $"HY{suffix}",
                nameVi = $"HSK test cập nhật {suffix}",
                sortOrder = 100
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadObjectAsync(update);
        updated.GetProperty("nameVi").GetString().Should().Contain("cập nhật");
        updated.GetProperty("sortOrder").GetInt32().Should().Be(100);

        var deactivate = await client.PostAsync($"/api/v1/admin/hsk-levels/{id}/deactivate", null);
        deactivate.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(deactivate)).GetProperty("isActive").GetBoolean().Should().BeFalse();

        var activate = await client.PostAsync($"/api/v1/admin/hsk-levels/{id}/activate", null);
        activate.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(activate)).GetProperty("isActive").GetBoolean().Should().BeTrue();

        var delete = await client.DeleteAsync($"/api/v1/admin/hsk-levels/{id}");
        delete.IsSuccessStatusCode.Should().BeTrue();

        // Soft-deleted HSK levels are intentionally hidden from the normal detail endpoint.
        var deletedDetail = await client.GetAsync($"/api/v1/admin/hsk-levels/{id}");
        deletedDetail.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var restore = await client.PostAsync($"/api/v1/admin/hsk-levels/{id}/restore", null);
        restore.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(restore)).GetProperty("isActive").GetBoolean().Should().BeTrue();

        var restoredDetail = await client.GetAsync($"/api/v1/admin/hsk-levels/{id}");
        restoredDetail.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await client.GetAsync("/api/v1/admin/hsk-levels");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadArrayAsync(list);
        items.Should().Contain(x => x.GetProperty("id").GetInt64() == id);
    }

    [Fact]
    public async Task Course_CreateReadUpdateDeleteAndRestore_Works()
    {
        var adminId = await CreateUserAsync("course-crud-admin");
        var client = Factory.CreateAdminClient(adminId);
        var hskId = await GetHsk1IdAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/courses",
            new
            {
                code = $"COURSE-{suffix}",
                slug = $"course-{suffix}",
                titleVi = $"Khóa học test {suffix}",
                shortDescriptionVi = "Mô tả ngắn",
                descriptionVi = "Mô tả chi tiết",
                hskLevelId = hskId,
                coverImageUrl = (string?)null,
                sortOrder = 10,
                estimatedMinutes = 120,
                isFeatured = false
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await ReadObjectAsync(create);
        var id = created.GetProperty("id").GetInt64();
        var token = created.GetProperty("concurrencyToken").GetGuid();

        var detail = await client.GetAsync($"/api/v1/admin/courses/{id}");
        detail.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/courses/{id}",
            new
            {
                code = $"COURSE-{suffix}",
                slug = $"course-{suffix}",
                titleVi = $"Khóa học đã sửa {suffix}",
                shortDescriptionVi = "Đã sửa",
                descriptionVi = "Đã sửa chi tiết",
                hskLevelId = hskId,
                coverImageUrl = (string?)null,
                sortOrder = 11,
                estimatedMinutes = 180,
                isFeatured = true,
                concurrencyToken = token
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadObjectAsync(update);
        updated.GetProperty("titleVi").GetString().Should().Contain("đã sửa");
        updated.GetProperty("isFeatured").GetBoolean().Should().BeTrue();
        var updatedToken = updated.GetProperty("concurrencyToken").GetGuid();

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/admin/courses/{id}")
        {
            Content = JsonContent.Create(new { concurrencyToken = updatedToken })
        };
        var delete = await client.SendAsync(deleteRequest);
        delete.IsSuccessStatusCode.Should().BeTrue();

        var deletedDetailResponse = await client.GetAsync($"/api/v1/admin/courses/{id}");
        deletedDetailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var deletedDetail = await ReadObjectAsync(deletedDetailResponse);
        deletedDetail.GetProperty("deletedAt").ValueKind.Should().NotBe(JsonValueKind.Null);
        var deletedToken = deletedDetail.GetProperty("concurrencyToken").GetGuid();

        var restore = await client.PostAsJsonAsync(
            $"/api/v1/admin/courses/{id}/restore-deleted",
            new { concurrencyToken = deletedToken });
        restore.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(restore)).GetProperty("deletedAt").ValueKind.Should().Be(JsonValueKind.Null);

        var listResponse = await client.GetAsync("/api/v1/admin/courses?page=1&pageSize=20");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await ReadObjectAsync(listResponse);
        list.TryGetProperty("total", out var total).Should().BeTrue();
        total.GetInt64().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Lesson_CreateReadUpdateDeleteAndRestore_Works()
    {
        var adminId = await CreateUserAsync("lesson-crud-admin");
        var client = Factory.CreateAdminClient(adminId);
        var hskId = await GetHsk1IdAsync();
        var suffix = Guid.NewGuid().ToString("N")[..8];

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/lessons",
            new
            {
                courseChapterId = (long?)null,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = $"lesson-{suffix}",
                titleVi = $"Bài giảng test {suffix}",
                shortDescriptionVi = "Mô tả ngắn",
                descriptionVi = "Mô tả",
                objectiveVi = "Mục tiêu",
                coverImageUrl = (string?)null,
                sortOrder = 0,
                estimatedMinutes = 15,
                difficulty = 1,
                isFeatured = false
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await ReadObjectAsync(create);
        var id = created.GetProperty("id").GetInt64();
        var version = created.GetProperty("version").GetInt32();

        var detail = await client.GetAsync($"/api/v1/admin/lessons/{id}");
        detail.StatusCode.Should().Be(HttpStatusCode.OK);

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/lessons/{id}",
            new
            {
                courseChapterId = (long?)null,
                hskLevelId = hskId,
                topicId = (long?)null,
                slug = $"lesson-{suffix}",
                titleVi = $"Bài giảng đã sửa {suffix}",
                shortDescriptionVi = "Đã sửa",
                descriptionVi = "Đã sửa mô tả",
                objectiveVi = "Đã sửa mục tiêu",
                coverImageUrl = (string?)null,
                sortOrder = 1,
                estimatedMinutes = 20,
                difficulty = 2,
                isFeatured = true,
                version
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadObjectAsync(update);
        updated.GetProperty("titleVi").GetString().Should().Contain("đã sửa");
        var updatedVersion = updated.GetProperty("version").GetInt32();

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/admin/lessons/{id}")
        {
            Content = JsonContent.Create(new { version = updatedVersion })
        };
        var delete = await client.SendAsync(deleteRequest);
        delete.IsSuccessStatusCode.Should().BeTrue();

        var deletedDetailResponse = await client.GetAsync($"/api/v1/admin/lessons/{id}");
        deletedDetailResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var deletedDetail = await ReadObjectAsync(deletedDetailResponse);
        deletedDetail.GetProperty("deletedAt").ValueKind.Should().NotBe(JsonValueKind.Null);
        var deletedVersion = deletedDetail.GetProperty("version").GetInt32();

        var restore = await client.PostAsJsonAsync(
            $"/api/v1/admin/lessons/{id}/restore-deleted",
            new { version = deletedVersion });
        restore.StatusCode.Should().Be(HttpStatusCode.OK);
        (await ReadObjectAsync(restore)).GetProperty("deletedAt").ValueKind.Should().Be(JsonValueKind.Null);

        var listResponse = await client.GetAsync("/api/v1/admin/lessons?page=1&pageSize=20");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await ReadObjectAsync(listResponse);
        list.TryGetProperty("total", out var total).Should().BeTrue();
        total.GetInt64().Should().BeGreaterThan(0);
    }

    private async Task<long> GetHsk1IdAsync()
        => await Factory.ExecuteDbAsync(async db =>
            await db.Set<HskLevel>()
                .Where(x => x.Code == "HSK1")
                .Select(x => x.Id)
                .SingleAsync());

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
}
