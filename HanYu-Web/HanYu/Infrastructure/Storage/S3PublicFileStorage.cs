using Amazon.S3;
using Amazon.S3.Model;
using HanYu.Application.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Storage;

public sealed class S3PublicFileStorage : IPublicFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly StorageOptions _options;

    public S3PublicFileStorage(
        IAmazonS3 s3,
        IOptions<StorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<PublicFileUploadResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.PublicBucketName))
            throw new InvalidOperationException("Storage:PublicBucketName chưa được cấu hình.");

        if (string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
            throw new InvalidOperationException("Storage:PublicBaseUrl chưa được cấu hình.");

        if (content.CanSeek)
            content.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.PublicBucketName,
            Key = objectKey,
            InputStream = content,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        var publicUrl = $"{_options.PublicBaseUrl.TrimEnd('/')}/{objectKey.TrimStart('/')}";
        return new PublicFileUploadResult(objectKey, publicUrl);
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        return _s3.DeleteObjectAsync(
            _options.PublicBucketName,
            objectKey,
            cancellationToken);
    }
}
