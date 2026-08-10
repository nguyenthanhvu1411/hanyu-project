using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HanYu.Infrastructure.BackgroundJobs.DataExport;

public sealed class UserDataExportWorker
    : BackgroundService
{
    private readonly IServiceScopeFactory
        _scopeFactory;

    private readonly ILogger<UserDataExportWorker>
        _logger;

    public UserDataExportWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<UserDataExportWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "User data export worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope =
                    _scopeFactory.CreateAsyncScope();

                var processor =
                    scope.ServiceProvider
                        .GetRequiredService<
                            UserDataExportProcessor>();

                var processed =
                    await processor.ProcessNextAsync(
                        stoppingToken);

                await processor.CleanupExpiredAsync(
                    stoppingToken);

                if (!processed)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(5),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken
                    .IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Unexpected export worker error.");

                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken);
            }
        }
    }
}
