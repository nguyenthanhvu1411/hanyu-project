using HanYu.Domain.Entities.Review;
using HanYu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Review;

using Common;

public sealed class ReviewIntegrationTests
    : IntegrationTestBase
{
    public ReviewIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task SubmitReview_UpdatesState_AndCreatesEvent()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var state =
            new UserVocabularyState(
                userId,
                data.VocabularyId);

        state.StartLearning(
            DateTimeOffset.UtcNow
                .AddHours(-2),
            60);

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(state);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateUserClient(
                userId);

        var response =
            await client.PostAsJsonAsync(
                "/api/v1/public/reviews",
                new
                {
                    vocabularyPublicId =
                        data.VocabularyPublicId,

                    rating =
                        ReviewRating.Good,

                    wasCorrect =
                        true,

                    responseTimeMs =
                        500
                });

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var result =
            await Factory.ExecuteDbAsync(
                async db =>
                {
                    var updated =
                        await db.Set<UserVocabularyState>()
                            .SingleAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.VocabularyId ==
                                    data.VocabularyId);

                    var eventCount =
                        await db.Set<ReviewEvent>()
                            .CountAsync(
                                x =>
                                    x.UserId ==
                                    userId &&
                                    x.VocabularyId ==
                                    data.VocabularyId);

                    return (
                        updated,
                        eventCount);
                });

        result.updated.CorrectCount
            .Should()
            .Be(1);

        result.updated.LastReviewedAt
            .Should()
            .NotBeNull();

        result.updated.NextReviewAt
            .Should()
            .NotBeNull();

        result.eventCount.Should().Be(1);
    }

    [Fact]
    public async Task ReviewQueue_ReturnsDueState()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var state =
            new UserVocabularyState(
                userId,
                data.VocabularyId);

        state.StartLearning(
            DateTimeOffset.UtcNow
                .AddHours(-3),
            60);

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(state);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateUserClient(
                userId);

        var response =
            await client.GetAsync(
                "/api/v1/public/review-queue");

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var body =
            await response.Content
                .ReadAsStringAsync();

        body.Should()
            .Contain(
                data.VocabularyPublicId
                    .ToString());
    }
}