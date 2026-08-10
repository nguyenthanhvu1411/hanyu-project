using HanYu.Domain.Enums;
using HanYu.Infrastructure.Review;

namespace HanYu.UnitTests.Review;

public sealed class ReviewSchedulerTests
{
    private readonly ReviewScheduler _scheduler =
        new();

    [Fact]
    public void GoodCorrect_IncreasesMastery()
    {
        var result =
            _scheduler.Calculate(
                30m,
                1440,
                ReviewRating.Good,
                true,
                DateTimeOffset.UtcNow);

        result.MasteryAfter
            .Should()
            .BeGreaterThan(30m);
    }

    [Fact]
    public void GoodCorrect_IncreasesInterval()
    {
        var result =
            _scheduler.Calculate(
                30m,
                1440,
                ReviewRating.Good,
                true,
                DateTimeOffset.UtcNow);

        result.IntervalAfterMinutes
            .Should()
            .BeGreaterThan(1440);
    }

    [Fact]
    public void Incorrect_DecreasesMastery()
    {
        var result =
            _scheduler.Calculate(
                70m,
                1440,
                ReviewRating.Easy,
                false,
                DateTimeOffset.UtcNow);

        result.MasteryAfter
            .Should()
            .BeLessThan(70m);
    }

    [Fact]
    public void Incorrect_ResetsToShortInterval()
    {
        var result =
            _scheduler.Calculate(
                70m,
                10080,
                ReviewRating.Easy,
                false,
                DateTimeOffset.UtcNow);

        result.IntervalAfterMinutes
            .Should()
            .BeLessThan(10080);
    }

    [Fact]
    public void Easy_HasLongerIntervalThanHard()
    {
        var now =
            DateTimeOffset.UtcNow;

        var hard =
            _scheduler.Calculate(
                50m,
                1440,
                ReviewRating.Hard,
                true,
                now);

        var easy =
            _scheduler.Calculate(
                50m,
                1440,
                ReviewRating.Easy,
                true,
                now);

        easy.IntervalAfterMinutes
            .Should()
            .BeGreaterThan(
                hard.IntervalAfterMinutes);
    }

    [Fact]
    public void Mastery_NeverExceeds100()
    {
        var result =
            _scheduler.Calculate(
                99m,
                1440,
                ReviewRating.Easy,
                true,
                DateTimeOffset.UtcNow);

        result.MasteryAfter.Should().Be(100m);
    }

    [Fact]
    public void Mastery_NeverBelowZero()
    {
        var result =
            _scheduler.Calculate(
                1m,
                1440,
                ReviewRating.Again,
                false,
                DateTimeOffset.UtcNow);

        result.MasteryAfter.Should().Be(0m);
    }
}
