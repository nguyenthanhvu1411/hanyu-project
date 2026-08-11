namespace HanYu.Application.Interfaces.Storage;

public sealed record PublicFileUploadResult(
    string ObjectKey,
    string PublicUrl);

public interface IPublicFileStorage
{
    Task<PublicFileUploadResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<string> GetReadUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default);
}
