using HanYu.Domain.Entities.Analytics;

namespace HanYu.UnitTests.Gamification;

public sealed class UserStreakTests
{
    [Fact]
    public void FirstLearningDay_StartsStreakAtOne()
    {
        var streak =
            new UserStreak(
                Guid.NewGuid());

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 1));

        streak.CurrentStreak.Should().Be(1);

        streak.LongestStreak.Should().Be(1);

        streak.TotalActiveDays.Should().Be(1);
    }

    [Fact]
    public void SameDay_DoesNotIncreaseStreak()
    {
        var streak =
            new UserStreak(
                Guid.NewGuid());

        var date =
            new DateOnly(2026, 8, 1);

        streak.RegisterLearningDay(date);

        streak.RegisterLearningDay(date);

        streak.CurrentStreak.Should().Be(1);

        streak.TotalActiveDays.Should().Be(1);
    }

    [Fact]
    public void ConsecutiveDay_IncreasesStreak()
    {
        var streak =
            new UserStreak(
                Guid.NewGuid());

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 1));

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 2));

        streak.CurrentStreak.Should().Be(2);

        streak.LongestStreak.Should().Be(2);

        streak.TotalActiveDays.Should().Be(2);
    }

    [Fact]
    public void MissingDay_ResetsCurrentStreak()
    {
        var streak =
            new UserStreak(
                Guid.NewGuid());

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 1));

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 2));

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 5));

        streak.CurrentStreak.Should().Be(1);

        streak.LongestStreak.Should().Be(2);

        streak.TotalActiveDays.Should().Be(3);
    }

    [Fact]
    public void OlderDate_ReturnsFalse()
    {
        var streak =
            new UserStreak(
                Guid.NewGuid());

        streak.RegisterLearningDay(
            new DateOnly(2026, 8, 5));

        var result = streak.RegisterLearningDay(
                new DateOnly(2026, 8, 4));

        result.Should().BeFalse();
    }
}
