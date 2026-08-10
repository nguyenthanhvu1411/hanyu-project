using HanYu.Domain.Entities.Notification;
using HanYu.Domain.Enums;

namespace HanYu.UnitTests.Notification;

public sealed class InAppNotificationTests
{
    [Fact]
    public void NewNotification_IsUnread()
    {
        var notification =
            Create();

        notification.IsRead.Should().BeFalse();

        notification.ReadAt.Should().BeNull();
    }

    [Fact]
    public void MarkRead_Works()
    {
        var notification =
            Create();

        notification.MarkRead();

        notification.IsRead.Should().BeTrue();

        notification.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkRead_IsIdempotent()
    {
        var notification =
            Create();

        notification.MarkRead();

        var first =
            notification.ReadAt;

        notification.MarkRead();

        notification.ReadAt.Should().Be(first);
    }

    [Fact]
    public void MarkUnread_Works()
    {
        var notification =
            Create();

        notification.MarkRead();

        notification.MarkUnread();

        notification.IsRead.Should().BeFalse();
    }

    [Fact]
    public void EmptyTitle_Throws()
    {
        var action =
            () => new InAppNotification(
                Guid.NewGuid(),
                default,
                "",
                "Message");

        action.Should()
            .Throw<ArgumentException>();
    }

    private static InAppNotification Create()
        => new(
            Guid.NewGuid(),
            default,
            "Thông báo",
            "Nội dung thông báo");
}
