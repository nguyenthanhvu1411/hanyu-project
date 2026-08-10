namespace HanYu.Application.Interfaces.Storage;

public interface IPrivateFileStorage
{
    Task<string> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<string> CreateDownloadUrlAsync(
        string objectKey,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default);
}
