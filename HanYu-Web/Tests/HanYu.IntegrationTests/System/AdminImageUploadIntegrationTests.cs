using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HanYu.IntegrationTests.System;

using Common;

public sealed class AdminImageUploadIntegrationTests : IntegrationTestBase
{
    public AdminImageUploadIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Admin_CanUploadImage()
    {
        var adminId = await CreateUserAsync("image-upload-admin");
        var client = Factory.CreateAdminClient(adminId);

        // Minimal valid 1x1 transparent PNG.
        var png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=");

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(png);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(fileContent, "file", "cover.png");

        var upload = await client.PostAsync("/api/v1/admin/uploads/images", form);
        upload.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await upload.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("contentType").GetString().Should().Be("image/png");
        root.GetProperty("size").GetInt64().Should().Be(png.Length);
        root.GetProperty("kind").GetString().Should().Be("image");
        root.GetProperty("fileName").GetString().Should().EndWith(".png");
        root.GetProperty("objectKey").GetString().Should().StartWith("images/");
        root.GetProperty("url").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AnonymousUser_CannotUploadImage()
    {
        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        form.Add(fileContent, "file", "cover.png");

        var response = await Factory.CreateAnonymousClient()
            .PostAsync("/api/v1/admin/uploads/images", form);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
