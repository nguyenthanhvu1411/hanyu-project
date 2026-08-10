using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HanYu.IntegrationTests.InfrastructureTests;

using Common;

public sealed class MigrationIntegrationTests
    : IntegrationTestBase
{
    public MigrationIntegrationTests(
        HanYuWebApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Database_CanConnect()
    {
        await using var scope =
            Factory.Services
                .CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    HanYuDbContext>();

        var canConnect =
            await db.Database
                .CanConnectAsync();

        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task Database_HasNoPendingMigrations()
    {
        await using var scope =
            Factory.Services
                .CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    HanYuDbContext>();

        var pending =
            await db.Database
                .GetPendingMigrationsAsync();

        pending.Should().BeEmpty();
    }
}