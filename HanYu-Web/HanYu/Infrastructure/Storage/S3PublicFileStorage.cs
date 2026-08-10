using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using HanYu.Application.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Storage;

public sealed class S3PublicFileStorage : IPublicFileStorage, IDisposable
{
    private readonly IAmazonS3 _s3;
    private readonly StorageOptions _options;

    public S3PublicFileStorage(IOptions<StorageOptions> options)
    {
        _options = options.Value;

        var config = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
            config.ForcePathStyle = _options.ForcePathStyle;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
        }

        _s3 = !string.IsNullOrWhiteSpace(_options.AccessKey) &&
              !string.IsNullOrWhiteSpace(_options.SecretKey)
            ? new AmazonS3Client(
                new BasicAWSCredentials(_options.AccessKey, _options.SecretKey),
                config)
            : new AmazonS3Client(config);
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

    public void Dispose()
    {
        _s3.Dispose();
    }
}
