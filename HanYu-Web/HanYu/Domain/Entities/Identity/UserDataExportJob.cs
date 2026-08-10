using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Identity;

public class UserDataExportJob : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public DataExportStatus Status { get; private set; }
        = DataExportStatus.Pending;

    public string? StoragePath { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public string? ErrorMessage { get; private set; }

    public User User { get; private set; } = null!;

    protected UserDataExportJob()
    {
    }

    public UserDataExportJob(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;
    }

    public void StartProcessing()
    {
        if (Status != DataExportStatus.Pending)
            throw new InvalidOperationException(
                "Export job chỉ có thể bắt đầu từ trạng thái Pending.");

        Status =
            DataExportStatus.Processing;

        ErrorMessage = null;

        MarkUpdated();
    }

    public void Complete(
        string storagePath,
        DateTimeOffset expiresAt)
    {
        if (Status != DataExportStatus.Processing)
            throw new InvalidOperationException(
                "Export job không ở trạng thái Processing.");

        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException(
                "StoragePath không được để trống.",
                nameof(storagePath));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentOutOfRangeException(
                nameof(expiresAt));

        StoragePath = storagePath.Trim();

        Status =
            DataExportStatus.Completed;

        CompletedAt =
            DateTimeOffset.UtcNow;

        ExpiresAt = expiresAt;
        ErrorMessage = null;

        MarkUpdated();
    }

    public void Fail(
        string errorMessage)
    {
        if (Status is DataExportStatus.Completed or
            DataExportStatus.Expired)
        {
            throw new InvalidOperationException(
                "Export job đã kết thúc.");
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException(
                "ErrorMessage không được để trống.",
                nameof(errorMessage));

        Status =
            DataExportStatus.Failed;

        ErrorMessage =
            errorMessage.Trim();

        CompletedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Expire()
    {
        if (Status != DataExportStatus.Completed)
            throw new InvalidOperationException(
                "Chỉ export job Completed mới có thể expire.");

        if (ExpiresAt.HasValue &&
            ExpiresAt.Value > DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException(
                "Export file chưa tới thời điểm hết hạn.");
        }

        Status =
            DataExportStatus.Expired;

        MarkUpdated();
    }
}
