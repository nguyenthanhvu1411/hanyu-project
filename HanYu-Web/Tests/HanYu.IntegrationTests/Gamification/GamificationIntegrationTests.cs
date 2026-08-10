using HanYu.Application.Interfaces.Gamification;
using HanYu.Domain.Entities.Gamification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanYu.IntegrationTests.Gamification;

using Common;

public sealed class GamificationIntegrationTests
    : IntegrationTestBase
{
    public GamificationIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AwardXp_SameSourceTwice_DoesNotDuplicate()
    {
        var userId =
            await CreateUserAsync();

        var sourceId =
            Guid.NewGuid()
                .ToString("N");

        await using var scope =
            Factory.Services
                .CreateAsyncScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<
                    IGamificationService>();

        var first =
            await service.AwardXpAsync(
                userId,
                20,
                "Integration",
                "lesson",
                sourceId);

        first.IsSuccess.Should().BeTrue();

        var second =
            await service.AwardXpAsync(
                userId,
                20,
                "Integration",
                "lesson",
                sourceId);

        second.IsSuccess.Should().BeTrue();

        var count =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<XpTransaction>()
                        .CountAsync(
                            x =>
                                x.UserId ==
                                userId &&
                                x.SourceType ==
                                "lesson" &&
                                x.SourceId ==
                                sourceId));

        count.Should().Be(1);
    }
}