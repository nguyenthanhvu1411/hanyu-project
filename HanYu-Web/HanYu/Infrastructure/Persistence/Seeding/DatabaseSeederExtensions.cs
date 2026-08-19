using HanYu.Infrastructure.Persistence.Seeding.Content;
using HanYu.Infrastructure.Persistence.Seeding.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        var identitySeeder =
            scope.ServiceProvider
                .GetRequiredService<IdentitySeeder>();

        await identitySeeder.SeedAsync(
            cancellationToken);

        // ContentTaxonomySeeder không cần đăng ký riêng trong DI vì mọi dependency
        // của nó (DbContext + ILogger) đã có sẵn trong scope hiện tại.
        var taxonomySeeder = ActivatorUtilities.CreateInstance<ContentTaxonomySeeder>(
            scope.ServiceProvider);

        await taxonomySeeder.SeedAsync(
            cancellationToken);
    }
}
