using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Content;

public class ContentImportJob : AuditableEntity
{
    public ContentImportType ImportType { get; private set; }

    public string OriginalFileName { get; private set; }
        = string.Empty;

    public string StoragePath { get; private set; }
        = string.Empty;

    public ContentImportStatus Status { get; private set; }
        = ContentImportStatus.Pending;

    public int TotalRows { get; private set; }

    public int ProcessedRows { get; private set; }

    public int SuccessRows { get; private set; }

    public int FailedRows { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public string? ErrorMessage { get; private set; }

    public ICollection<ContentImportRow> Rows { get; private set; }
        = new List<ContentImportRow>();

    protected ContentImportJob()
    {
    }

    public ContentImportJob(
        ContentImportType importType,
        string originalFileName,
        string storagePath)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException(
                "Tên file gốc không được để trống.",
                nameof(originalFileName));

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException(
                "StoragePath không được để trống.",
                nameof(storagePath));

        ImportType = importType;
        OriginalFileName = originalFileName.Trim();
        StoragePath = storagePath.Trim();
    }

    public void UpdateSource(
        string originalFileName,
        string storagePath)
    {
        if (Status != ContentImportStatus.Pending)
            throw new InvalidOperationException(
                "Chỉ có thể cập nhật file nguồn khi job đang Pending.");

        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException(
                "Tên file gốc không được để trống.",
                nameof(originalFileName));

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException(
                "StoragePath không được để trống.",
                nameof(storagePath));

        OriginalFileName = originalFileName.Trim();
        StoragePath = storagePath.Trim();

        MarkUpdated();
    }

    public void Start(int totalRows)
    {
        if (Status != ContentImportStatus.Pending)
            throw new InvalidOperationException(
                "Import job chỉ có thể bắt đầu từ trạng thái Pending.");

        if (totalRows < 0)
            throw new ArgumentOutOfRangeException(
                nameof(totalRows));

        TotalRows = totalRows;
        ProcessedRows = 0;
        SuccessRows = 0;
        FailedRows = 0;
        ErrorMessage = null;

        Status = ContentImportStatus.Processing;
        StartedAt = DateTimeOffset.UtcNow;
        CompletedAt = null;

        MarkUpdated();
    }

    public void RegisterProcessedRow(bool succeeded)
    {
        if (Status != ContentImportStatus.Processing)
            throw new InvalidOperationException(
                "Import job không ở trạng thái Processing.");

        if (TotalRows > 0 &&
            ProcessedRows >= TotalRows)
        {
            throw new InvalidOperationException(
                "Số dòng đã xử lý không thể vượt quá TotalRows.");
        }

        ProcessedRows++;

        if (succeeded)
            SuccessRows++;
        else
            FailedRows++;

        MarkUpdated();
    }

    public void Complete()
    {
        if (Status != ContentImportStatus.Processing)
            throw new InvalidOperationException(
                "Chỉ job đang Processing mới có thể hoàn tất.");

        if (TotalRows > 0 &&
            ProcessedRows != TotalRows)
        {
            throw new InvalidOperationException(
                "Chưa xử lý hết dữ liệu import.");
        }

        Status = FailedRows > 0
            ? ContentImportStatus.CompletedWithErrors
            : ContentImportStatus.Completed;

        CompletedAt = DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Fail(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException(
                "Lỗi import không được để trống.",
                nameof(errorMessage));

        if (Status is ContentImportStatus.Completed or
            ContentImportStatus.CompletedWithErrors)
        {
            throw new InvalidOperationException(
                "Job đã hoàn tất không thể chuyển sang Failed.");
        }

        Status = ContentImportStatus.Failed;
        ErrorMessage = errorMessage.Trim();
        CompletedAt = DateTimeOffset.UtcNow;

        MarkUpdated();
    }
}