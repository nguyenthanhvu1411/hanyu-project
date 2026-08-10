using HanYu.Domain.Entities.Quiz;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Quiz;

using Common;

public sealed class QuizIntegrationTests
    : IntegrationTestBase
{
    public QuizIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task PublishedQuiz_IsPublic()
    {
        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateAnonymousClient();

        var response =
            await client.GetAsync(
                $"/api/v1/public/quizzes/{data.QuizPublicId}");

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task StartAttempt_SameIdempotencyKey_ReturnsSameAttempt()
    {
        var userId =
            await CreateUserAsync();

        var data =
            await TestDataSeeder
                .SeedLearningDataAsync(
                    Factory);

        var client =
            Factory.CreateUserClient(
                userId);

        var key =
            Guid.NewGuid()
                .ToString("N");

        var first =
            await client.PostAsJsonAsync(
                $"/api/v1/public/quizzes/{data.QuizPublicId}/attempts",
                new
                {
                    idempotencyKey = key
                });

        first.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var second =
            await client.PostAsJsonAsync(
                $"/api/v1/public/quizzes/{data.QuizPublicId}/attempts",
                new
                {
                    idempotencyKey = key
                });

        second.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var count =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<QuizAttempt>()
                        .CountAsync(
                            x =>
                                x.UserId ==
                                userId &&
                                x.QuizId ==
                                data.QuizId &&
                                x.IdempotencyKey ==
                                key));

        count.Should().Be(1);
    }
}