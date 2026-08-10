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
