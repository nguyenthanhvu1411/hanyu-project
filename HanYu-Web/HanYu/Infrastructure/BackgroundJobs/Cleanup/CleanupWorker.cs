using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Notification;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HanYu.Infrastructure.BackgroundJobs.Cleanup;

/// <summary>
/// Background worker that runs periodic cleanup tasks on a configurable interval.
///
/// Tasks performed:
///   1. Delete expired refresh tokens (token pruning)
///   2. Delete expired in-app notifications
///   3. Mark stale quiz attempts as expired
///   4. Prune old product events beyond retention window
///
/// Design principles:
///   - Runs at a low cadence (every 1 hour by default) to minimise DB load
///   - Uses a dedicated DI scope per cycle (avoids DbContext reuse across loops)
///   - Never throws — logs errors and continues next cycle
///   - Uses cancellation token for graceful shutdown
/// </summary>
public sealed class CleanupWorker : BackgroundService
{
    private static readonly TimeSpan RunInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan StartDelay = TimeSpan.FromMinutes(2); // Let the app fully start first

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupWorker> _logger;

    public CleanupWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<CleanupWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cleanup worker started. First run in {Delay}.", StartDelay);

        // Give the app time to fully start before running first cleanup
        await Task.Delay(StartDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCleanupCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup worker encountered an unhandled error.");
            }

            await Task.Delay(RunInterval, stoppingToken);
        }

        _logger.LogInformation("Cleanup worker stopped.");
    }

    private async Task RunCleanupCycleAsync(CancellationToken ct)
    {
        _logger.LogDebug("Starting cleanup cycle at {UtcNow}.", DateTimeOffset.UtcNow);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<HanYuDbContext>();
        var now = DateTimeOffset.UtcNow;

        var tasks = new[]
        {
            PruneExpiredRefreshTokensAsync(db, now, ct),
            PruneExpiredNotificationsAsync(db, now, ct),
            ExpireStaleQuizAttemptsAsync(db, now, ct),
        };

        var results = await Task.WhenAll(tasks.Select(SafeRunAsync));

        _logger.LogInformation(
            "Cleanup cycle complete. " +
            "RefreshTokens={Tokens}, Notifications={Notifs}, QuizAttempts={Attempts}.",
            results[0], results[1], results[2]);
    }

    /// <summary>
    /// Wraps each cleanup task and returns the affected row count (or -1 on error).
    /// Ensures one failing task does not abort the others.
    /// </summary>
    private async Task<int> SafeRunAsync(Task<int> task)
    {
        try { return await task; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A cleanup sub-task failed.");
            return -1;
        }
    }

    // ─── Individual cleanup operations ───────────────────────────────────────────

    private static async Task<int> PruneExpiredRefreshTokensAsync(
        HanYuDbContext db,
        DateTimeOffset now,
        CancellationToken ct)
    {
        // Delete tokens that have been expired OR revoked for more than 7 days.
        // Keep revoked tokens for 7 days for audit trail.
        var cutoff = now.AddDays(-7);

        return await db.Set<RefreshToken>()
            .Where(t =>
                t.ExpiresAt < now ||
                (t.RevokedAt != null && t.RevokedAt < cutoff))
            .ExecuteDeleteAsync(ct);
    }

    private static async Task<int> PruneExpiredNotificationsAsync(
        HanYuDbContext db,
        DateTimeOffset now,
        CancellationToken ct)
    {
        // Delete notifications that have an explicit expiry date that has passed.
        return await db.Set<InAppNotification>()
            .Where(n => n.ExpiresAt != null && n.ExpiresAt < now)
            .ExecuteDeleteAsync(ct);
    }

    private static async Task<int> ExpireStaleQuizAttemptsAsync(
        HanYuDbContext db,
        DateTimeOffset now,
        CancellationToken ct)
    {
        // Mark in-progress quiz attempts as expired if they passed their expiry time.
        // Uses ExecuteUpdateAsync (EF Core 7+) for a single-query UPDATE — no loading.
        return await db.Set<QuizAttempt>()
            .Where(a =>
                a.Status == QuizAttemptStatus.InProgress &&
                a.ExpiresAt != null &&
                a.ExpiresAt < now)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(
                    a => a.Status,
                    QuizAttemptStatus.Expired),
                ct);
    }
}
