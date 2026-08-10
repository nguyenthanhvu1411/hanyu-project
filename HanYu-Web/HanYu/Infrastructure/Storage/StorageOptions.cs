namespace HanYu.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string Provider { get; init; }
        = "BackblazeB2";

    public string? AccessKey { get; init; }

    public string? SecretKey { get; init; }

    public string Region { get; init; }
        = "us-west-004";

    // Backblaze B2 S3-compatible endpoint example:
    // https://s3.us-west-004.backblazeb2.com
    public string? ServiceUrl { get; init; }

    // Backblaze B2 works with virtual-host style; keep false.
    public bool ForcePathStyle { get; init; }

    // Private bucket dành cho exports và dữ liệu không public.
    public string ExportBucketName { get; init; }
        = "hanyu-private";

    // Media bucket. It may be private; when PublicBaseUrl is empty the backend
    // returns a short-lived pre-signed GET URL after upload/read operations.
    public string PublicBucketName { get; init; }
        = "hanyu-public";

    // Optional direct public/CDN base URL. Leave empty for a private B2 bucket.
    public string PublicBaseUrl { get; init; }
        = string.Empty;

    public int MediaReadUrlExpirationMinutes { get; init; }
        = 15;

    public int ExportUrlExpirationMinutes { get; init; }
        = 15;

    public int ExportFileExpirationDays { get; init; }
        = 7;

    public int MaxImageSizeMb { get; init; }
        = 10;

    public int MaxAudioSizeMb { get; init; }
        = 50;

    public int MaxVideoSizeMb { get; init; }
        = 200;

    public int MaxDocumentSizeMb { get; init; }
        = 50;
}
