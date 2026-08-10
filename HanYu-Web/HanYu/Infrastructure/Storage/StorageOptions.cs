namespace HanYu.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string? AccessKey { get; init; }

    public string? SecretKey { get; init; }

    public string Region { get; init; }
        = "ap-southeast-1";

    // Để trống nếu dùng AWS S3.
    // Điền nếu dùng Supabase Storage S3, R2, MinIO hoặc S3-compatible khác.
    public string? ServiceUrl { get; init; }

    public bool ForcePathStyle { get; init; }

    // Private bucket dành cho exports và dữ liệu không public.
    public string ExportBucketName { get; init; }
        = "hanyu-exports";

    // Public bucket dành cho cover/image assets.
    public string PublicBucketName { get; init; }
        = "hanyu-public";

    // Public CDN/base URL của bucket public.
    // Ví dụ Supabase:
    // https://PROJECT.supabase.co/storage/v1/object/public/hanyu-public
    public string PublicBaseUrl { get; init; }
        = string.Empty;

    public int ExportUrlExpirationMinutes { get; init; }
        = 15;

    public int ExportFileExpirationDays { get; init; }
        = 7;
}
