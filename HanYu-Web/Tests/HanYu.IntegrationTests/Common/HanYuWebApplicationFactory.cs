using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace HanYu.IntegrationTests.Common;

public sealed class HanYuWebApplicationFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    public HanYuWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("Jwt__SecretKey", "HanYu-Integration-Test-Secret-Key-0123456789-ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "HanYu.IntegrationTests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "HanYu.IntegrationTests");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationMinutes", "30");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationDays", "7");
    }

    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder()
            .WithImage(
                "postgres:16-alpine")
            .WithDatabase(
                "hanyu_integration")
            .WithUsername(
                "postgres")
            .WithPassword(
                "postgres")
            .Build();

    private Respawner? _respawner;

    public string ConnectionString =>
        _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        /*
         * Force WebApplication startup.
         */
        _ = Server;

        await using var scope =
            Services.CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    HanYuDbContext>();

        await db.Database.MigrateAsync();

        await CreateRespawnerAsync();
    }

    public new async Task DisposeAsync()
    {
        Dispose();

        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment(
            "IntegrationTest");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                var values =
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            ConnectionString,

                        ["Jwt:SecretKey"] =
                            "HanYu-Integration-Test-Secret-Key-0123456789-ABCDEFGHIJKLMNOPQRSTUVWXYZ",

                        ["Jwt:Issuer"] =
                            "HanYu.IntegrationTests",

                        ["Jwt:Audience"] =
                            "HanYu.IntegrationTests",

                        ["Jwt:AccessTokenExpirationMinutes"] =
                            "30",

                        ["Jwt:RefreshTokenExpirationDays"] =
                            "7"
                    };

                configuration.AddInMemoryCollection(
                    values);
            });

        builder.ConfigureServices(
            services =>
            {
                services.RemoveAll<
                    DbContextOptions<HanYuDbContext>>();

                services.RemoveAll<
                    HanYuDbContext>();

                services.AddDbContext<
                    HanYuDbContext>(
                    options =>
                    {
                        options
                            .UseNpgsql(
                                ConnectionString)
                            .UseSnakeCaseNamingConvention();
                    });

                services
                    .AddAuthentication(
                        options =>
                        {
                            options.DefaultAuthenticateScheme =
                                TestAuthenticationHandler.Scheme;

                            options.DefaultChallengeScheme =
                                TestAuthenticationHandler.Scheme;

                            options.DefaultScheme =
                                TestAuthenticationHandler.Scheme;
                        })
                    .AddScheme<
                        AuthenticationSchemeOptions,
                        TestAuthenticationHandler>(
                        TestAuthenticationHandler.Scheme,
                        _ =>
                        {
                        });

                services.AddSingleton<
                    IAuthorizationHandler,
                    TestAuthorizationHandler>();
            });
    }

    public HttpClient CreateAnonymousClient()
        => CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

    public HttpClient CreateUserClient(
        Guid userId)
    {
        var client =
            CreateAnonymousClient();

        client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.UserIdHeader,
            userId.ToString());

        return client;
    }

    public HttpClient CreateAdminClient(
        Guid userId)
    {
        var client =
            CreateUserClient(userId);

        client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.RoleHeader,
            "Admin");

        client.DefaultRequestHeaders.Add(
            TestAuthenticationHandler.AllowAllHeader,
            "true");

        return client;
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null)
        {
            await CreateRespawnerAsync();
        }

        await using var connection =
            new NpgsqlConnection(
                ConnectionString);

        await connection.OpenAsync();

        await _respawner!.ResetAsync(
            connection);
    }

    public async Task ExecuteDbAsync(
        Func<HanYuDbContext, Task> action)
    {
        await using var scope =
            Services.CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    HanYuDbContext>();

        await action(db);
    }

    public async Task<T> ExecuteDbAsync<T>(
        Func<HanYuDbContext, Task<T>> action)
    {
        await using var scope =
            Services.CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    HanYuDbContext>();

        return await action(db);
    }

    private async Task CreateRespawnerAsync()
    {
        await using var connection =
            new NpgsqlConnection(
                ConnectionString);

        await connection.OpenAsync();

        _respawner =
            await Respawner.CreateAsync(
                connection,
                new RespawnerOptions
                {
                    DbAdapter =
                        DbAdapter.Postgres,

                    SchemasToInclude =
                        ["public"],

                    TablesToIgnore =
                    [
                        new Table(
                            "__EFMigrationsHistory")
                    ]
                });
    }
}