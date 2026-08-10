using System.Net;
using HanYu.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;

namespace HanYu.IntegrationTests.Notification;

using Common;

public sealed class NotificationIntegrationTests
    : IntegrationTestBase
{
    public NotificationIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task User_CanReadOwnNotifications()
    {
        var userId =
            await CreateUserAsync();

        var notification =
            new InAppNotification(
                userId,
                default,
                "Integration Notification",
                "Hello");

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(notification);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateUserClient(
                userId);

        var response =
            await client.GetAsync(
                "/api/v1/public/notifications");

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var body =
            await response.Content
                .ReadAsStringAsync();

        body.Should()
            .Contain(
                "Integration Notification");
    }

    [Fact]
    public async Task User_CannotModifyOtherUsersNotification()
    {
        var ownerId =
            await CreateUserAsync(
                "owner");

        var attackerId =
            await CreateUserAsync(
                "attacker");

        var notification =
            new InAppNotification(
                ownerId,
                default,
                "Private",
                "Private");

        await Factory.ExecuteDbAsync(
            async db =>
            {
                db.Add(notification);

                await db.SaveChangesAsync();
            });

        var client =
            Factory.CreateUserClient(
                attackerId);

        var response =
            await client.PatchAsync(
                $"/api/v1/public/notifications/{notification.PublicId}/read",
                null);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.Forbidden);

        var readAt =
            await Factory.ExecuteDbAsync(
                db =>
                    db.Set<InAppNotification>()
                        .Where(
                            x =>
                                x.Id ==
                                notification.Id)
                        .Select(x => x.ReadAt)
                        .SingleAsync());

        readAt.Should().BeNull();
    }
}