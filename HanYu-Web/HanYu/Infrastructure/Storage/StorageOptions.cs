namespace HanYu.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    // Backblaze B2 S3-compatible credentials:
    // AccessKey = Application Key ID
    // SecretKey = Application Key
    // Never commit real credentials to appsettings*.json.
    public string? AccessKey { get; init; }

    public string? SecretKey { get; init; }

    // Use the exact B2 region shown for the bucket, e.g. us-west-004.
    public string Region { get; init; }
        = "us-west-004";

    // Backblaze B2 S3 endpoint format:
    // https://s3.<region>.backblazeb2.com
    // Also works with other S3-compatible providers.
    public string? ServiceUrl { get; init; }

    // Backblaze B2 supports both virtual-hosted and path-style requests.
    // Keep false by default; enable only for providers that require path style.
    public bool ForcePathStyle { get; init; }

    // Private bucket dành cho exports và dữ liệu không public.
    public string ExportBucketName { get; init; }
        = "hanyu-exports";

    // Public bucket dành cho media public: cover, ảnh, audio, video, documents.
    public string PublicBucketName { get; init; }
        = "hanyu-public";

    // Browser-facing base URL for the public bucket.
    // For B2 this can be the bucket's friendly/public download base URL.
    // Keep it configurable so a CDN/custom domain can replace it later
    // without changing database schema or application code.
    public string PublicBaseUrl { get; init; }
        = string.Empty;

    public int ExportUrlExpirationMinutes { get; init; }
        = 15;

    public int ExportFileExpirationDays { get; init; }
        = 7;
}
