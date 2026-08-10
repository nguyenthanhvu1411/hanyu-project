using Amazon.S3;
using Amazon.S3.Model;
using HanYu.Application.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Storage;

public sealed class S3PrivateFileStorage
    : IPrivateFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly StorageOptions _options;

    public S3PrivateFileStorage(
        IAmazonS3 s3,
        IOptions<StorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (content.CanSeek)
            content.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.ExportBucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);
        return objectKey;
    }

    public async Task<string> CreateDownloadUrlAsync(
        string objectKey,
        TimeSpan lifetime,
        CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.ExportBucketName,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(lifetime)
        };

        return await _s3.GetPreSignedURLAsync(request);
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        return _s3.DeleteObjectAsync(
            _options.ExportBucketName,
            objectKey,
            cancellationToken);
    }
}
