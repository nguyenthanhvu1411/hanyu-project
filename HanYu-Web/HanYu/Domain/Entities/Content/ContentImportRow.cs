using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Content;

public class ContentImportRow : BaseEntity
{
    public long ImportJobId { get; private set; }

    public int RowNumber { get; private set; }

    public string SourceJson { get; private set; }
        = string.Empty;

    public bool IsSuccessful { get; private set; }

    public long? CreatedEntityId { get; private set; }

    public string? ErrorCode { get; private set; }

    public string? ErrorMessage { get; private set; }

    public DateTimeOffset ProcessedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public ContentImportJob ImportJob { get; private set; }
        = null!;

    protected ContentImportRow()
    {
    }

    public ContentImportRow(
        long importJobId,
        int rowNumber,
        string sourceJson)
    {
        if (importJobId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(importJobId));

        if (rowNumber <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(rowNumber));

        if (string.IsNullOrWhiteSpace(sourceJson))
            throw new ArgumentException(
                "SourceJson không được để trống.",
                nameof(sourceJson));

        ImportJobId = importJobId;
        RowNumber = rowNumber;
        SourceJson = sourceJson;
    }

    public void MarkSuccess(
        long? createdEntityId = null)
    {
        if (createdEntityId.HasValue &&
            createdEntityId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(createdEntityId));
        }

        IsSuccessful = true;
        CreatedEntityId = createdEntityId;
        ErrorCode = null;
        ErrorMessage = null;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    public void MarkFailed(
        string? errorCode,
        string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException(
                "ErrorMessage không được để trống.",
                nameof(errorMessage));

        IsSuccessful = false;
        CreatedEntityId = null;

        ErrorCode = string.IsNullOrWhiteSpace(errorCode)
            ? null
            : errorCode.Trim();

        ErrorMessage = errorMessage.Trim();

        ProcessedAt = DateTimeOffset.UtcNow;
    }
}