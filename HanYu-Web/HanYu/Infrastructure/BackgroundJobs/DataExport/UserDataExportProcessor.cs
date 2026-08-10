using HanYu.Application.Interfaces.Storage;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HanYu.Infrastructure.Storage;

namespace HanYu.Infrastructure.BackgroundJobs.DataExport;

public sealed class UserDataExportProcessor
{
    private readonly HanYuDbContext _dbContext;
    private readonly IIdentityDataExportBuilder _builder;
    private readonly IPrivateFileStorage _storage;
    private readonly StorageOptions _storageOptions;
    private readonly ILogger<UserDataExportProcessor> _logger;

    public UserDataExportProcessor(
        HanYuDbContext dbContext,
        IIdentityDataExportBuilder builder,
        IPrivateFileStorage storage,
        IOptions<StorageOptions> storageOptions,
        ILogger<UserDataExportProcessor> logger)
    {
        _dbContext = dbContext;
        _builder = builder;
        _storage = storage;
        _storageOptions = storageOptions.Value;
        _logger = logger;
    }

    public async Task<bool> ProcessNextAsync(
        CancellationToken cancellationToken)
    {
        var job =
            await _dbContext
                .Set<UserDataExportJob>()
                .Where(x =>
                    x.Status ==
                    DataExportStatus.Pending)
                .OrderBy(x =>
                    x.RequestedAt)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (job is null)
            return false;

        try
        {
            job.StartProcessing();

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await using var exportStream =
                await _builder.BuildAsync(
                    job.UserId,
                    cancellationToken);

            var objectKey =
                $"user-exports/{job.UserId:N}/" +
                $"{Guid.NewGuid():N}.zip";

            var storagePath =
                await _storage.UploadAsync(
                    objectKey,
                    exportStream,
                    "application/zip",
                    cancellationToken);

            var expiresAt =
                DateTimeOffset.UtcNow.AddDays(
                    _storageOptions
                        .ExportFileExpirationDays);

            job.Complete(
                storagePath,
                expiresAt);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            _logger.LogInformation(
                "User data export completed for UserId={UserId}",
                job.UserId);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "User data export failed for UserId={UserId}",
                job.UserId);

            // DbContext có thể đang track state lỗi;
            // reload job trước khi Fail.
            _dbContext.ChangeTracker.Clear();

            var failedJob =
                await _dbContext
                    .Set<UserDataExportJob>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == job.Id,
                        cancellationToken);

            if (failedJob is not null)
            {
                failedJob.Fail(
                    "Không thể tạo file export.");

                await _dbContext.SaveChangesAsync(
                    cancellationToken);
            }

            return true;
        }
    }

    public async Task CleanupExpiredAsync(
        CancellationToken cancellationToken)
    {
        var jobs =
            await _dbContext
                .Set<UserDataExportJob>()
                .Where(x =>
                    x.Status ==
                        DataExportStatus.Completed &&
                    x.ExpiresAt.HasValue &&
                    x.ExpiresAt.Value <=
                        DateTimeOffset.UtcNow)
                .Take(50)
                .ToListAsync(
                    cancellationToken);

        foreach (var job in jobs)
        {
            if (!string.IsNullOrWhiteSpace(
                    job.StoragePath))
            {
                try
                {
                    await _storage.DeleteAsync(
                        job.StoragePath,
                        cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Failed deleting expired export for UserId={UserId}",
                        job.UserId);

                    continue;
                }
            }

            job.Expire();
        }

        if (jobs.Count > 0)
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}
