using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Vocabulary;

using Common;

public sealed class VocabularyAudioCommandIntegrationTests
    : IntegrationTestBase
{
    public VocabularyAudioCommandIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task VocabularyAudioCommand_EnforcesKindDetachAndVersion()
    {
        var adminId = await CreateUserAsync("vocabulary-audio-admin");
        var seed = await CreateSeedAsync();
        var client = Factory.CreateAdminClient(adminId);

        var wrongKind = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = seed.ExampleAudioId,
                version = seed.Version
            });

        wrongKind.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadCodeAsync(wrongKind)).Should().Be("AudioAsset.InvalidKind");

        var attach = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = seed.VocabularyAudioId,
                version = seed.Version
            });

        attach.StatusCode.Should().Be(HttpStatusCode.OK);
        var attached = await ReadObjectAsync(attach);
        attached.GetProperty("audioAssetId").GetInt64().Should().Be(seed.VocabularyAudioId);
        var attachedVersion = attached.GetProperty("version").GetInt32();
        attachedVersion.Should().Be(seed.Version + 1);

        var detach = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = (long?)null,
                version = attachedVersion
            });

        detach.StatusCode.Should().Be(HttpStatusCode.OK);
        var detached = await ReadObjectAsync(detach);
        detached.GetProperty("audioAssetId").ValueKind.Should().Be(JsonValueKind.Null);
        var detachedVersion = detached.GetProperty("version").GetInt32();

        var staleVersion = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = seed.VocabularyAudioId,
                version = seed.Version
            });

        staleVersion.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ReadCodeAsync(staleVersion)).Should().Be("Vocabulary.VersionConflict");

        var missing = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = 9_999_999L,
                version = detachedVersion
            });

        missing.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadCodeAsync(missing)).Should().Be("AudioAsset.NotFound");

        var archived = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/audio",
            new
            {
                audioAssetId = seed.ArchivedVocabularyAudioId,
                version = detachedVersion
            });

        archived.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ReadCodeAsync(archived)).Should().Be("AudioAsset.Archived");
    }

    [Fact]
    public async Task ExampleAudioCommand_EnforcesKindAndSupportsDetach()
    {
        var adminId = await CreateUserAsync("example-audio-admin");
        var seed = await CreateSeedAsync();
        var client = Factory.CreateAdminClient(adminId);

        var wrongKind = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/examples/{seed.ExampleId}/audio",
            new { audioAssetId = seed.VocabularyAudioId });

        wrongKind.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadCodeAsync(wrongKind)).Should().Be("AudioAsset.InvalidKind");

        var attach = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/examples/{seed.ExampleId}/audio",
            new { audioAssetId = seed.ExampleAudioId });

        attach.StatusCode.Should().Be(HttpStatusCode.OK);
        var attached = await ReadObjectAsync(attach);
        attached.GetProperty("audioAssetId").GetInt64().Should().Be(seed.ExampleAudioId);

        var detach = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/examples/{seed.ExampleId}/audio",
            new { audioAssetId = (long?)null });

        detach.StatusCode.Should().Be(HttpStatusCode.OK);
        var detached = await ReadObjectAsync(detach);
        detached.GetProperty("audioAssetId").ValueKind.Should().Be(JsonValueKind.Null);

        var missing = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/examples/{seed.ExampleId}/audio",
            new { audioAssetId = 9_999_999L });

        missing.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await ReadCodeAsync(missing)).Should().Be("AudioAsset.NotFound");

        var archived = await client.PutAsJsonAsync(
            $"/api/v1/admin/vocabularies/{seed.VocabularyId}/examples/{seed.ExampleId}/audio",
            new { audioAssetId = seed.ArchivedExampleAudioId });

        archived.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await ReadCodeAsync(archived)).Should().Be("AudioAsset.Archived");
    }

    private async Task<AudioSeed> CreateSeedAsync()
    {
        var hskLevelId = await Factory.ExecuteDbAsync(
            db => db.Set<HskLevel>()
                .AsNoTracking()
                .Where(item => item.Code == "HSK1")
                .Select(item => item.Id)
                .SingleAsync());

        var vocabulary = new Domain.Entities.Vocabulary.Vocabulary(
            hskLevelId,
            $"测{Guid.NewGuid():N}"[..8],
            "cè shì",
            $"ce-shi-{Guid.NewGuid():N}",
            "kiểm tra");

        var vocabularyAudio = new AudioAsset(
            $"audio/vocabulary/{Guid.NewGuid():N}.mp3",
            AudioAssetKind.Vocabulary,
            "audio/mpeg");
        var exampleAudio = new AudioAsset(
            $"audio/example/{Guid.NewGuid():N}.mp3",
            AudioAssetKind.ExampleSentence,
            "audio/mpeg");
        var archivedVocabularyAudio = new AudioAsset(
            $"audio/vocabulary/{Guid.NewGuid():N}.mp3",
            AudioAssetKind.Vocabulary,
            "audio/mpeg");
        var archivedExampleAudio = new AudioAsset(
            $"audio/example/{Guid.NewGuid():N}.mp3",
            AudioAssetKind.ExampleSentence,
            "audio/mpeg");

        archivedVocabularyAudio.Archive();
        archivedExampleAudio.Archive();

        await Factory.ExecuteDbAsync(async db =>
        {
            db.Add(vocabulary);
            db.Add(vocabularyAudio);
            db.Add(exampleAudio);
            db.Add(archivedVocabularyAudio);
            db.Add(archivedExampleAudio);
            await db.SaveChangesAsync();
        });

        var example = new VocabularyExample(
            vocabulary.Id,
            "这是一个例子。",
            "zhè shì yí ge lì zi",
            "Đây là một ví dụ.",
            1);

        await Factory.ExecuteDbAsync(async db =>
        {
            db.Add(example);
            await db.SaveChangesAsync();
        });

        return new AudioSeed(
            vocabulary.Id,
            vocabulary.Version,
            example.Id,
            vocabularyAudio.Id,
            exampleAudio.Id,
            archivedVocabularyAudio.Id,
            archivedExampleAudio.Id);
    }

    private static async Task<JsonElement> ReadObjectAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    private static async Task<string?> ReadCodeAsync(HttpResponseMessage response)
    {
        var body = await ReadObjectAsync(response);
        return body.TryGetProperty("code", out var code)
            ? code.GetString()
            : null;
    }

    private sealed record AudioSeed(
        long VocabularyId,
        int Version,
        long ExampleId,
        long VocabularyAudioId,
        long ExampleAudioId,
        long ArchivedVocabularyAudioId,
        long ArchivedExampleAudioId);
}
