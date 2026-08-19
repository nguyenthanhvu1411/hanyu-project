using System.Net;
using System.Text.Json;
using HanYu.IntegrationTests.Common;

namespace HanYu.IntegrationTests.Content;

public sealed class MediaUploadIntegrationTests : IntegrationTestBase
{
    public MediaUploadIntegrationTests(HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Theory]
    [InlineData("/api/v1/admin/uploads/images", "cover.png", "image/png", "image")]
    [InlineData("/api/v1/admin/uploads/audio", "lesson.mp3", "audio/mpeg", "audio")]
    [InlineData("/api/v1/admin/uploads/videos", "lesson.mp4", "video/mp4", "video")]
    [InlineData("/api/v1/admin/uploads/documents", "lesson.pdf", "application/pdf", "document")]
    public async Task Admin_CanUploadSupportedMedia(
        string endpoint,
        string fileName,
        string contentType,
        string expectedKind)
    {
        var adminId = await CreateUserAsync("media-admin");
        var client = Factory.CreateAdminClient(adminId);

        using var request = new MultipartFormDataContent();
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6 };
        using var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new global::System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        request.Add(file, "file", fileName);

        var response = await client.PostAsync(endpoint, request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var result = document.RootElement;

        result.GetProperty("url").GetString().Should().StartWith("https://storage.test/");
        result.GetProperty("objectKey").GetString().Should().NotBeNullOrWhiteSpace();
        result.GetProperty("fileName").GetString().Should().NotBeNullOrWhiteSpace();
        result.GetProperty("contentType").GetString().Should().Be(contentType);
        result.GetProperty("size").GetInt64().Should().Be(bytes.Length);
        result.GetProperty("kind").GetString().Should().Be(expectedKind);
    }

    [Fact]
    public async Task Upload_RejectsMismatchedUnsupportedMimeType()
    {
        var adminId = await CreateUserAsync("media-admin-invalid");
        var client = Factory.CreateAdminClient(adminId);

        using var request = new MultipartFormDataContent();
        using var file = new ByteArrayContent(new byte[] { 1, 2, 3 });
        file.Headers.ContentType = new global::System.Net.Http.Headers.MediaTypeHeaderValue("application/x-msdownload");
        request.Add(file, "file", "malware.exe");

        var response = await client.PostAsync("/api/v1/admin/uploads/images", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AnonymousUser_CannotUploadMedia()
    {
        var client = Factory.CreateAnonymousClient();

        using var request = new MultipartFormDataContent();
        using var file = new ByteArrayContent(new byte[] { 1, 2, 3 });
        file.Headers.ContentType = new global::System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        request.Add(file, "file", "cover.png");

        var response = await client.PostAsync("/api/v1/admin/uploads/images", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
