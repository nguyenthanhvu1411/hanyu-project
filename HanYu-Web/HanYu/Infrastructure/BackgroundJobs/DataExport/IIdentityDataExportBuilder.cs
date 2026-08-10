namespace HanYu.Infrastructure.BackgroundJobs.DataExport;

public interface IIdentityDataExportBuilder
{
    Task<Stream> BuildAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
