using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Entities.Analytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanYu.IntegrationTests.Analytics;

using Common;

public sealed class AnalyticsIntegrationTests
    : IntegrationTestBase
{
    public AnalyticsIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Review_CreatesDailyAnalytics()
    {
        var userId =
            await CreateUserAsync();

        await using var scope =
            Factory.Services
                .CreateAsyncScope();

        var collector =
            scope.ServiceProvider
                .GetRequiredService<
                    ILearningAnalyticsCollector>();

        await collector.RegisterReviewAsync(
            userId,
            true);

        var stat =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<DailyLearningStat>()
                        .SingleAsync(
                            x =>
                                x.UserId ==
                                userId));

        stat.VocabularyReviewed
            .Should()
            .Be(1);

        stat.CorrectReviews
            .Should()
            .Be(1);

        stat.WrongReviews
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task QuizPass_IncrementsQuizCounters()
    {
        var userId =
            await CreateUserAsync();

        await using var scope =
            Factory.Services
                .CreateAsyncScope();

        var collector =
            scope.ServiceProvider
                .GetRequiredService<
                    ILearningAnalyticsCollector>();

        await collector.RegisterQuizAttemptAsync(
            userId,
            true);

        var stat =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<DailyLearningStat>()
                        .SingleAsync(
                            x =>
                                x.UserId ==
                                userId));

        stat.QuizAttempts.Should().Be(1);

        stat.QuizPassed.Should().Be(1);
    }
}