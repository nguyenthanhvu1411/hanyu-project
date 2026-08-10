using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace HanYu.IntegrationTests.Common;

public abstract class IntegrationTestBase
    : IClassFixture<HanYuWebApplicationFactory>,
      IAsyncLifetime
{
    protected HanYuWebApplicationFactory Factory
    {
        get;
    }

    protected IntegrationTestBase(
        HanYuWebApplicationFactory factory)
    {
        Factory =
            factory;
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();

        await TestDataSeeder
            .SeedReferenceDataAsync(
                Factory);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;

    protected async Task<Guid> CreateUserAsync(
        string? prefix = null)
    {
        await using var scope =
            Factory.Services.CreateAsyncScope();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<
                    UserManager<User>>();

        var suffix =
            Guid.NewGuid()
                .ToString("N");

        var userName =
            $"{prefix ?? "test"}_{suffix}";

        var email =
            $"{userName}@example.com";

        var user =
            new User(
                userName,
                email);

        var result =
            await userManager.CreateAsync(
                user,
                "TestPassword123!");

        result.Succeeded
            .Should()
            .BeTrue(
                string.Join(
                    "; ",
                    result.Errors.Select(
                        x => x.Description)));

        return user.Id;
    }

    protected static string Unique(
        string prefix)
        => $"{prefix}-{Guid.NewGuid():N}";
}