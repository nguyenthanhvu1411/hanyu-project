using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.DataPrivacy;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Storage;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HanYu.Infrastructure.Storage;

namespace HanYu.Infrastructure.Identity;

public sealed class DataPrivacyService
    : IDataPrivacyService
{
    private readonly HanYuDbContext _dbContext;
    private readonly IPrivateFileStorage _storage;
    private readonly StorageOptions _storageOptions;

    public DataPrivacyService(
        HanYuDbContext dbContext,
        IPrivateFileStorage storage,
        IOptions<StorageOptions> storageOptions)
    {
        _dbContext = dbContext;
        _storage = storage;
        _storageOptions = storageOptions.Value;
    }

    public async Task<
        Result<IReadOnlyCollection<ConsentResponse>>>
        GetConsentsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var exists =
            await _dbContext
                .Set<User>()
                .AnyAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (!exists)
        {
            return Result.Failure<
                IReadOnlyCollection<ConsentResponse>>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var consents =
            await _dbContext
                .Set<UserConsent>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .OrderBy(x => x.ConsentType)
                .Select(x =>
                    new ConsentResponse(
                        x.ConsentType,
                        x.Version,
                        x.IsGranted,
                        x.GrantedAt,
                        x.RevokedAt))
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<ConsentResponse>>(
            consents);
    }

    public async Task<Result<ConsentResponse>>
        UpdateConsentAsync(
            Guid userId,
            UpdateConsentRequest request,
            CancellationToken cancellationToken = default)
    {
        var userExists =
            await _dbContext
                .Set<User>()
                .AnyAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (!userExists)
        {
            return Result.Failure<ConsentResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var consent =
            await _dbContext
                .Set<UserConsent>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.ConsentType ==
                            request.ConsentType,
                    cancellationToken);

        try
        {
            if (consent is null)
            {
                consent =
                    new UserConsent(
                        userId,
                        request.ConsentType,
                        request.Version,
                        request.IsGranted);

                _dbContext
                    .Set<UserConsent>()
                    .Add(consent);
            }
            else if (request.IsGranted)
            {
                if (!string.Equals(
                        consent.Version,
                        request.Version,
                        StringComparison.Ordinal))
                {
                    consent.AcceptNewVersion(
                        request.Version);
                }
                else
                {
                    consent.Grant();
                }
            }
            else
            {
                consent.Revoke();
            }
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<ConsentResponse>(
                Error.Validation(
                    "Identity.InvalidConsent",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            MapConsent(consent));
    }

    public async Task<Result<DataExportResponse>>
        RequestExportAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var userExists =
            await _dbContext
                .Set<User>()
                .AnyAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (!userExists)
        {
            return Result.Failure<DataExportResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var activeJob =
            await _dbContext
                .Set<UserDataExportJob>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        (
                            x.Status ==
                                DataExportStatus.Pending ||
                            x.Status ==
                                DataExportStatus.Processing
                        ),
                    cancellationToken);

        if (activeJob is not null)
        {
            return Result.Failure<DataExportResponse>(
                Error.Conflict(
                    "Identity.DataExportAlreadyRunning",
                    "Đã có một yêu cầu xuất dữ liệu đang xử lý."));
        }

        var job =
            new UserDataExportJob(userId);

        _dbContext
            .Set<UserDataExportJob>()
            .Add(job);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            MapExport(job));
    }

    public async Task<
        Result<IReadOnlyCollection<DataExportResponse>>>
        GetExportsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var jobs =
            await _dbContext
                .Set<UserDataExportJob>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId)
                .OrderByDescending(x =>
                    x.RequestedAt)
                .Select(x =>
                    new DataExportResponse(
                        x.Status,
                        x.RequestedAt,
                        x.CompletedAt,
                        x.ExpiresAt,
                        x.Status == DataExportStatus.Completed && x.ExpiresAt > DateTimeOffset.UtcNow,
                        x.ErrorMessage))
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<DataExportResponse>>(
            jobs);
    }

    public async Task<
        Result<DataExportDownloadResponse>>
        GetLatestExportDownloadAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var job =
            await _dbContext
                .Set<UserDataExportJob>()
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.Status ==
                        DataExportStatus.Completed)
                .OrderByDescending(
                    x => x.CompletedAt)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (job is null ||
            string.IsNullOrWhiteSpace(
                job.StoragePath))
        {
            return Result.Failure<
                DataExportDownloadResponse>(
                    Error.NotFound(
                        "Identity.DataExportNotFound",
                        "Không có file export khả dụng."));
        }

        if (!job.ExpiresAt.HasValue ||
            job.ExpiresAt.Value <=
                DateTimeOffset.UtcNow)
        {
            return Result.Failure<
                DataExportDownloadResponse>(
                    Error.Validation(
                        "Identity.DataExportExpired",
                        "File export đã hết hạn."));
        }

        var lifetime =
            TimeSpan.FromMinutes(
                _storageOptions
                    .ExportUrlExpirationMinutes);

        var url =
            await _storage
                .CreateDownloadUrlAsync(
                    job.StoragePath,
                    lifetime,
                    cancellationToken);

        return Result.Success(
            new DataExportDownloadResponse(
                url,
                DateTimeOffset.UtcNow.Add(
                    lifetime)));
    }

    private static ConsentResponse MapConsent(
        UserConsent consent)
    {
        return new ConsentResponse(
            consent.ConsentType,
            consent.Version,
            consent.IsGranted,
            consent.GrantedAt,
            consent.RevokedAt);
    }

    private static DataExportResponse MapExport(
        UserDataExportJob job)
    {
        return new DataExportResponse(
            job.Status,
            job.RequestedAt,
            job.CompletedAt,
            job.ExpiresAt,
            job.Status == DataExportStatus.Completed &&
            job.ExpiresAt >
                DateTimeOffset.UtcNow,
            job.ErrorMessage);
    }
}
