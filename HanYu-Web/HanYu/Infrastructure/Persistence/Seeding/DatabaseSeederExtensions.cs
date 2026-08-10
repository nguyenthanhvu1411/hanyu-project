using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HanYu.Infrastructure.Persistence.Seeding.Identity;

namespace HanYu.Infrastructure.Persistence.Seeding;

public static class DatabaseSeederExtensions
{
    public static async Task SeedHanYuDatabaseAsync(
        this WebApplication app,
        CancellationToken cancellationToken = default)
    {
        // Tuyệt đối không tự chạy development seed
        // trong Production.
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        await using var scope =
            app.Services.CreateAsyncScope();

        var seeder =
            scope.ServiceProvider
                .GetRequiredService<
                    IdentitySeeder>();

        await seeder.SeedAsync(
            cancellationToken);
    }
}
