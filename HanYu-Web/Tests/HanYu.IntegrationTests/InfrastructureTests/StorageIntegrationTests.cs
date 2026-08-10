using System.Net;
using System.Text.Json;
using HanYu.IntegrationTests.Common;

namespace HanYu.IntegrationTests.InfrastructureTests;

public sealed class StorageIntegrationTests : IntegrationTestBase
{
    public StorageIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Admin_CanVerifyStorageConnectivity()
    {
        var adminId = await CreateUserAsync("storage-admin");
        var client = Factory.CreateAdminClient(adminId);

        var response = await client.PostAsync(
            "/api/v1/admin/storage/verify",
            content: null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync());

        var root = document.RootElement;
        root.GetProperty("success").GetBoolean().Should().BeTrue();
        root.GetProperty("bucketName").GetString().Should().NotBeNullOrWhiteSpace();
        root.GetProperty("objectKey").GetString().Should().StartWith("healthchecks/");
        root.GetProperty("readUrl").GetString().Should().StartWith("https://storage.test/");
    }

    [Fact]
    public async Task Anonymous_CannotVerifyStorageConnectivity()
    {
        var response = await Factory.CreateAnonymousClient().PostAsync(
            "/api/v1/admin/storage/verify",
            content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
