using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Enums;

namespace HanYu.UnitTests.Lesson;

public sealed class LessonTests
{
    [Fact]
    public void Constructor_CreatesDraftLesson()
    {
        var lesson =
            CreateLesson();

        lesson.Status.Should().Be(
            ContentStatus.Draft);

        lesson.HskLevelId.Should().Be(1);

        lesson.Slug.Should().Be(
            "bài-học-đầu-tiên");

        lesson.TitleVi.Should().Be(
            "Bài học đầu tiên");
    }

    [Fact]
    public void Lifecycle_DraftToPublished_Works()
    {
        var lesson =
            CreateLesson();

        lesson.SubmitForReview();

        lesson.Status.Should().Be(
            ContentStatus.Review);

        lesson.Approve();

        lesson.Status.Should().Be(
            ContentStatus.Approved);

        lesson.Publish();

        lesson.Status.Should().Be(
            ContentStatus.Published);

        lesson.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_FromDraft_Throws()
    {
        var lesson =
            CreateLesson();

        var action =
            () => lesson.Approve();

        action.Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void InvalidHskLevel_Throws()
    {
        var action =
            () =>
                new Domain.Entities.Lesson.Lesson(
                    0,
                    "invalid",
                    "Invalid");

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void UpdateCore_InvalidDifficulty_Throws()
    {
        var lesson =
            CreateLesson();

        var action =
            () => lesson.UpdateCore(
                1,
                "test",
                "Test",
                null,
                null,
                null,
                0,
                15,
                6);

        action.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    private static Domain.Entities.Lesson.Lesson
        CreateLesson()
        => new(
            1,
            "Bài Học Đầu Tiên",
            "Bài học đầu tiên");
}
