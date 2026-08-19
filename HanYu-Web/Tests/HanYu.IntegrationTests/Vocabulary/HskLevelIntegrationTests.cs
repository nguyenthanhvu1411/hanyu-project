using System.Net;
using System.Text.Json;

namespace HanYu.IntegrationTests.Vocabulary;

using Common;

public sealed class HskLevelIntegrationTests : IntegrationTestBase
{
    public HskLevelIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Admin_CanCreateAndListHskLevel()
    {
        var adminId = await CreateUserAsync("hsk-admin");
        var client = Factory.CreateAdminClient(adminId);

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/hsk-levels",
            new
            {
                code = "HSK7",
                nameVi = "HSK 7 - Nâng cao",
                sortOrder = 7
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createdDocument = JsonDocument.Parse(
            await create.Content.ReadAsStringAsync());
        var created = createdDocument.RootElement;

        created.GetProperty("id").GetInt64().Should().BeGreaterThan(0);
        created.GetProperty("publicId").GetGuid().Should().NotBe(Guid.Empty);
        created.GetProperty("code").GetString().Should().Be("HSK7");
        created.GetProperty("nameVi").GetString().Should().Be("HSK 7 - Nâng cao");

        var list = await client.GetAsync("/api/v1/admin/hsk-levels");
        list.StatusCode.Should().Be(HttpStatusCode.OK);

        using var listDocument = JsonDocument.Parse(
            await list.Content.ReadAsStringAsync());
        var items = listDocument.RootElement.EnumerateArray().ToArray();

        items.Should().Contain(x =>
            x.GetProperty("code").GetString() == "HSK7" &&
            x.GetProperty("nameVi").GetString() == "HSK 7 - Nâng cao");
    }

    [Fact]
    public async Task Admin_CanUpdateOnlyHskName_AndToggleStatus()
    {
        var adminId = await CreateUserAsync("hsk-update-admin");
        var client = Factory.CreateAdminClient(adminId);

        var create = await client.PostAsJsonAsync(
            "/api/v1/admin/hsk-levels",
            new
            {
                code = "HSK8",
                nameVi = "HSK 8 - Tên cũ",
                sortOrder = 8
            });

        create.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createdDocument = JsonDocument.Parse(
            await create.Content.ReadAsStringAsync());
        var id = createdDocument.RootElement.GetProperty("id").GetInt64();

        var update = await client.PutAsJsonAsync(
            $"/api/v1/admin/hsk-levels/{id}",
            new
            {
                code = "HSK8",
                nameVi = "HSK 8 - Tên mới",
                sortOrder = 8
            });

        update.StatusCode.Should().Be(HttpStatusCode.OK);

        using var updatedDocument = JsonDocument.Parse(
            await update.Content.ReadAsStringAsync());
        var updated = updatedDocument.RootElement;
        updated.GetProperty("code").GetString().Should().Be("HSK8");
        updated.GetProperty("nameVi").GetString().Should().Be("HSK 8 - Tên mới");
        updated.GetProperty("isActive").GetBoolean().Should().BeTrue();

        var deactivate = await client.PostAsync(
            $"/api/v1/admin/hsk-levels/{id}/deactivate",
            content: null);

        deactivate.StatusCode.Should().Be(HttpStatusCode.OK);

        using var deactivatedDocument = JsonDocument.Parse(
            await deactivate.Content.ReadAsStringAsync());
        deactivatedDocument.RootElement
            .GetProperty("isActive")
            .GetBoolean()
            .Should()
            .BeFalse();

        var activate = await client.PostAsync(
            $"/api/v1/admin/hsk-levels/{id}/activate",
            content: null);

        activate.StatusCode.Should().Be(HttpStatusCode.OK);

        using var activatedDocument = JsonDocument.Parse(
            await activate.Content.ReadAsStringAsync());
        activatedDocument.RootElement
            .GetProperty("isActive")
            .GetBoolean()
            .Should()
            .BeTrue();

        var list = await client.GetAsync("/api/v1/admin/hsk-levels");
        list.StatusCode.Should().Be(HttpStatusCode.OK);

        using var listDocument = JsonDocument.Parse(
            await list.Content.ReadAsStringAsync());

        listDocument.RootElement
            .EnumerateArray()
            .Should()
            .Contain(x =>
                x.GetProperty("id").GetInt64() == id &&
                x.GetProperty("nameVi").GetString() == "HSK 8 - Tên mới" &&
                x.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task AnonymousUser_CannotCreateHskLevel()
    {
        var response = await Factory.CreateAnonymousClient().PostAsJsonAsync(
            "/api/v1/admin/hsk-levels",
            new
            {
                code = "HSK7",
                nameVi = "HSK 7",
                sortOrder = 7
            });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
