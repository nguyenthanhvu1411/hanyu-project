using HanYu.Domain.Entities.Gamification;

namespace HanYu.UnitTests.Gamification;

public sealed class AchievementTests
{
    [Fact]
    public void Constructor_NormalizesCode()
    {
        var achievement =
            new Achievement(
                " first_lesson ",
                "Bài học đầu tiên",
                20);

        achievement.Code
            .Should()
            .Be("FIRST_LESSON");

        achievement.XpReward.Should().Be(20);
    }

    [Fact]
    public void Deactivate_Works()
    {
        var achievement =
            new Achievement(
                "TEST",
                "Test");

        achievement.Deactivate();

        achievement.IsActive.Should().BeFalse();
    }

    [Fact]
    public void NegativeReward_Throws()
    {
        var action =
            () =>
                new Achievement(
                    "INVALID",
                    "Invalid",
                    -1);

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }
}
