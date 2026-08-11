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

        if (string.IsNullOrWhiteSpace(_options.AccessKey) ||
            string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException(
                "Storage credentials chưa được cấu hình. Hãy đặt Storage:AccessKey và Storage:SecretKey bằng user-secrets hoặc environment variables.");
        }

        var config = new AmazonS3Config
        {
            SignatureVersion = "4"
        };

        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
            config.ForcePathStyle = _options.ForcePathStyle;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
        }

        _s3 = new AmazonS3Client(
            new BasicAWSCredentials(_options.AccessKey, _options.SecretKey),
            config);
    }

    public async Task<PublicFileUploadResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        EnsureBucketConfigured();

        if (content.CanSeek)
            content.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.PublicBucketName,
            Key = NormalizeObjectKey(objectKey),
            InputStream = content,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        return new PublicFileUploadResult(
            request.Key,
            await GetReadUrlAsync(request.Key, cancellationToken));
    }

    public Task<string> GetReadUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureBucketConfigured();

        var normalizedKey = NormalizeObjectKey(objectKey);

        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
        {
            return Task.FromResult(
                $"{_options.PublicBaseUrl.TrimEnd('/')}/{normalizedKey}");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(
            Math.Clamp(_options.MediaReadUrlExpirationMinutes, 1, 60 * 24));

        var url = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.PublicBucketName,
            Key = normalizedKey,
            Verb = HttpVerb.GET,
            Expires = expiresAt
        });

        return Task.FromResult(url);
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        EnsureBucketConfigured();

        return _s3.DeleteObjectAsync(
            _options.PublicBucketName,
            NormalizeObjectKey(objectKey),
            cancellationToken);
    }

    private void EnsureBucketConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.PublicBucketName))
        {
            throw new InvalidOperationException(
                "Storage:PublicBucketName chưa được cấu hình.");
        }
    }

    private static string NormalizeObjectKey(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            throw new ArgumentException("Object key không được để trống.", nameof(objectKey));

        return objectKey.Trim().TrimStart('/');
    }

    public void Dispose()
    {
        _s3.Dispose();
    }
}
