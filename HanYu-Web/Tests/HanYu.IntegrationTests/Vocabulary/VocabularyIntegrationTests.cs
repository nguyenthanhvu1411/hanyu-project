using System.Net;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Vocabulary;

using Common;

public sealed class VocabularyIntegrationTests
    : IntegrationTestBase
{
    public VocabularyIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task DraftVocabulary_IsNotPublic()
    {
        var word =
            new Domain.Entities.Vocabulary.Vocabulary(
                1,
                "测试",
                "cè shì",
                "ce4 shi4",
                "kiểm tra");

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(word);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/vocabularies/{Uri.EscapeDataString(word.Simplified)}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PublishedVocabulary_IsPublic()
    {
        var word =
            new Domain.Entities.Vocabulary.Vocabulary(
                1,
                "学习",
                "xué xí",
                "xue2 xi2",
                "học tập");

        word.SubmitForReview();

        word.Approve();

        word.Publish();

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(word);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/vocabularies/{Uri.EscapeDataString(word.Simplified)}");

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var body =
            await response.Content
                .ReadAsStringAsync();

        body.Should()
            .Contain("学习");
    }
}