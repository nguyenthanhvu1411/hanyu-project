namespace HanYu.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string BucketName { get; init; }
        = string.Empty;

    public string Region { get; init; }
        = "ap-southeast-1";

    // Để trống nếu dùng AWS S3.
    // Điền nếu dùng R2/MinIO/S3-compatible.
    public string? ServiceUrl { get; init; }

    public bool ForcePathStyle { get; init; }

    public int ExportUrlExpirationMinutes { get; init; }
        = 15;

    public int ExportFileExpirationDays { get; init; }
        = 7;
}
