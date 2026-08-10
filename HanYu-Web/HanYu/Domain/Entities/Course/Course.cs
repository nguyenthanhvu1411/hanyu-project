using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Course;

public sealed class Course : AuditableEntity
{
    public string Code { get; private set; }
        = string.Empty;

    public string Slug { get; private set; }
        = string.Empty;

    public string TitleVi { get; private set; }
        = string.Empty;

    public string? ShortDescriptionVi { get; private set; }

    public string? DescriptionVi { get; private set; }

    public long? HskLevelId { get; private set; }

    public HskLevel? HskLevel { get; private set; }

    public string? CoverImageUrl { get; private set; }

    public int SortOrder { get; private set; }

    public int? EstimatedMinutes { get; private set; }

    public ContentStatus Status { get; private set; }
        = ContentStatus.Draft;

    public bool IsActive { get; private set; }
        = true;

    public bool IsFeatured { get; private set; }

    public DateTimeOffset? PublishedAt { get; private set; }

    public Guid? PublishedById { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    public Guid? ArchivedById { get; private set; }

    public Guid ConcurrencyToken { get; private set; }
        = Guid.NewGuid();

    public ICollection<CourseChapter> Chapters { get; private set; }
        = new List<CourseChapter>();

    public ICollection<CoursePrerequisite> Prerequisites { get; private set; }
        = new List<CoursePrerequisite>();

    public ICollection<CoursePrerequisite> RequiredByCourses { get; private set; }
        = new List<CoursePrerequisite>();

    private Course()
    {
    }

    public Course(
        string code,
        string slug,
        string titleVi,
        long? hskLevelId,
        int sortOrder = 0,
        string? shortDescriptionVi = null,
        string? descriptionVi = null,
        string? coverImageUrl = null,
        int? estimatedMinutes = null)
    {
        SetCode(code);
        SetSlug(slug);
        SetTitleVi(titleVi);
        SetHskLevel(hskLevelId);
        SetSortOrder(sortOrder);
        SetShortDescription(shortDescriptionVi);
        SetDescription(descriptionVi);
        SetCoverImage(coverImageUrl);
        SetEstimatedMinutes(estimatedMinutes);

        Status = ContentStatus.Draft;
        IsActive = true;
        IsFeatured = false;
    }

    public void Update(
        string code,
        string slug,
        string titleVi,
        long? hskLevelId,
        int sortOrder,
        string? shortDescriptionVi,
        string? descriptionVi,
        string? coverImageUrl,
        int? estimatedMinutes,
        Guid updatedById)
    {
        EnsureEditable();

        SetCode(code);
        SetSlug(slug);
        SetTitleVi(titleVi);
        SetHskLevel(hskLevelId);
        SetSortOrder(sortOrder);
        SetShortDescription(shortDescriptionVi);
        SetDescription(descriptionVi);
        SetCoverImage(coverImageUrl);
        SetEstimatedMinutes(estimatedMinutes);

        Touch(updatedById);
    }

    public void Activate(Guid updatedById)
    {
        EnsureNotDeleted();

        if (IsActive)
            return;

        IsActive = true;

        Touch(updatedById);
    }

    public void Deactivate(Guid updatedById)
    {
        EnsureNotDeleted();

        if (!IsActive)
            return;

        IsActive = false;

        Touch(updatedById);
    }

    public void SetFeatured(
        bool featured,
        Guid updatedById)
    {
        EnsureNotDeleted();

        if (IsFeatured == featured)
            return;

        IsFeatured = featured;

        Touch(updatedById);
    }

    public void ChangeOrder(
        int sortOrder,
        Guid updatedById)
    {
        EnsureEditable();

        SetSortOrder(sortOrder);

        Touch(updatedById);
    }

    public void ChangeHskLevel(
        long? hskLevelId,
        Guid updatedById)
    {
        EnsureEditable();

        SetHskLevel(hskLevelId);

        Touch(updatedById);
    }

    public void ChangeCover(
        string? coverImageUrl,
        Guid updatedById)
    {
        EnsureEditable();

        SetCoverImage(coverImageUrl);

        Touch(updatedById);
    }

    // ============================================================
    // SUBMIT FOR REVIEW
    // =========================================================

    public void SubmitForReview(
        Guid updatedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            updatedById,
            nameof(updatedById));

        if (Status != ContentStatus.Draft)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học ở trạng thái Draft mới có thể gửi duyệt.");
        }

        ValidateReadyForReview();

        Status = ContentStatus.Review;

        Touch(updatedById);
    }

    // =========================================================
    // APPROVE
    // =========================================================

    public void Approve(
        Guid approvedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            approvedById,
            nameof(approvedById));

        if (Status != ContentStatus.Review)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học đang Review mới có thể được duyệt.");
        }

        /*
         * Rule reviewer != creator nên tiếp tục kiểm tra
         * ở Application Service.
         */

        Status = ContentStatus.Approved;

        Touch(approvedById);
    }

    // =========================================================
    // REJECT
    // =========================================================

    public void Reject(
        string reason,
        Guid rejectedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            rejectedById,
            nameof(rejectedById));

        if (Status != ContentStatus.Review)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học đang Review mới có thể bị từ chối.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "Lý do từ chối không được để trống.",
                nameof(reason));
        }

        /*
         * Nếu muốn lưu reason lâu dài:
         * nên lưu vào review/audit entity,
         * không nhét trực tiếp vào Course.
         */

        Status = ContentStatus.Draft;

        Touch(rejectedById);
    }

    // =========================================================
    // PUBLISH
    // =========================================================

    public void Publish(
        Guid publishedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            publishedById,
            nameof(publishedById));

        if (!IsActive)
        {
            throw new InvalidOperationException(
                "Không thể xuất bản khóa học đang ngừng hoạt động.");
        }

        if (Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học đã Approved mới có thể Publish.");
        }

        ValidatePublishable();

        Status = ContentStatus.Published;

        PublishedAt = DateTimeOffset.UtcNow;
        PublishedById = publishedById;

        ArchivedAt = null;
        ArchivedById = null;

        Touch(publishedById);
    }

    // =========================================================
    // ARCHIVE
    // =========================================================

    public void Archive(
        Guid archivedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            archivedById,
            nameof(archivedById));

        if (Status != ContentStatus.Published)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học Published mới có thể Archive.");
        }

        Status = ContentStatus.Archived;

        ArchivedAt = DateTimeOffset.UtcNow;
        ArchivedById = archivedById;

        IsActive = false;

        Touch(archivedById);
    }

    // =========================================================
    // RESTORE TO DRAFT
    // =========================================================

    public void RestoreToDraft(
        Guid updatedById)
    {
        EnsureNotDeleted();

        ValidateUserId(
            updatedById,
            nameof(updatedById));

        if (Status != ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học Archived mới có thể restore.");
        }

        Status = ContentStatus.Draft;

        PublishedAt = null;
        PublishedById = null;

        ArchivedAt = null;
        ArchivedById = null;

        IsActive = true;

        Touch(updatedById);
    }

    public void Delete(Guid deletedById)
    {
        ValidateUserId(
            deletedById,
            nameof(deletedById));

        if (IsDeleted)
            return;

        SoftDelete(deletedById);

        IsActive = false;

        RefreshConcurrencyToken();
    }

    public void RestoreDeleted(Guid restoredById)
    {
        ValidateUserId(
            restoredById,
            nameof(restoredById));

        if (!IsDeleted)
            return;

        Restore(restoredById);

        Status = ContentStatus.Draft;

        IsActive = true;

        PublishedAt = null;
        PublishedById = null;

        ArchivedAt = null;
        ArchivedById = null;

        RefreshConcurrencyToken();
    }

    public void EnsureConcurrencyToken(
        Guid concurrencyToken)
    {
        if (
            concurrencyToken == Guid.Empty ||
            ConcurrencyToken != concurrencyToken)
        {
            throw new InvalidOperationException(
                "Dữ liệu khóa học đã được thay đổi bởi người khác.");
        }
    }

    private void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException(
                "Mã khóa học không được để trống.",
                nameof(code));
        }

        code =
            code.Trim()
                .ToUpperInvariant();

        if (code.Length > 50)
        {
            throw new ArgumentException(
                "Mã khóa học không được vượt quá 50 ký tự.",
                nameof(code));
        }

        Code = code;
    }

    private void SetSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException(
                "Slug khóa học không được để trống.",
                nameof(slug));
        }

        slug = NormalizeSlug(slug);

        if (slug.Length > 160)
        {
            throw new ArgumentException(
                "Slug khóa học không được vượt quá 160 ký tự.",
                nameof(slug));
        }

        Slug = slug;
    }

    private void SetTitleVi(string titleVi)
    {
        if (string.IsNullOrWhiteSpace(titleVi))
        {
            throw new ArgumentException(
                "Tên khóa học không được để trống.",
                nameof(titleVi));
        }

        titleVi = titleVi.Trim();

        if (titleVi.Length > 200)
        {
            throw new ArgumentException(
                "Tên khóa học không được vượt quá 200 ký tự.",
                nameof(titleVi));
        }

        TitleVi = titleVi;
    }

    private void SetShortDescription(
        string? shortDescriptionVi)
    {
        shortDescriptionVi =
            Normalize(shortDescriptionVi);

        if (shortDescriptionVi?.Length > 500)
        {
            throw new ArgumentException(
                "Mô tả ngắn không được vượt quá 500 ký tự.",
                nameof(shortDescriptionVi));
        }

        ShortDescriptionVi = shortDescriptionVi;
    }

    private void SetDescription(
        string? descriptionVi)
    {
        descriptionVi =
            Normalize(descriptionVi);

        if (descriptionVi?.Length > 8000)
        {
            throw new ArgumentException(
                "Mô tả khóa học không được vượt quá 8000 ký tự.",
                nameof(descriptionVi));
        }

        DescriptionVi = descriptionVi;
    }

    private void SetHskLevel(
        long? hskLevelId)
    {
        if (
            hskLevelId.HasValue &&
            hskLevelId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hskLevelId),
                "HSK level không hợp lệ.");
        }

        HskLevelId = hskLevelId;
    }

    private void SetCoverImage(
        string? coverImageUrl)
    {
        coverImageUrl =
            Normalize(coverImageUrl);

        if (coverImageUrl?.Length > 2048)
        {
            throw new ArgumentException(
                "CoverImageUrl quá dài.",
                nameof(coverImageUrl));
        }

        CoverImageUrl = coverImageUrl;
    }

    private void SetSortOrder(
        int sortOrder)
    {
        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder),
                "SortOrder không được âm.");
        }

        SortOrder = sortOrder;
    }

    private void SetEstimatedMinutes(
        int? estimatedMinutes)
    {
        if (
            estimatedMinutes.HasValue &&
            estimatedMinutes.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(estimatedMinutes),
                "EstimatedMinutes phải lớn hơn 0.");
        }

        EstimatedMinutes = estimatedMinutes;
    }

    // =========================================================
    // VALIDATION
    // =========================================================

    private void ValidateReadyForReview()
    {
        if (string.IsNullOrWhiteSpace(Code))
        {
            throw new InvalidOperationException(
                "Khóa học chưa có mã.");
        }

        if (string.IsNullOrWhiteSpace(Slug))
        {
            throw new InvalidOperationException(
                "Khóa học chưa có Slug.");
        }

        if (string.IsNullOrWhiteSpace(TitleVi))
        {
            throw new InvalidOperationException(
                "Khóa học chưa có tên.");
        }

        if (!Chapters.Any(
                x =>
                    !x.IsDeleted &&
                    x.IsActive))
        {
            throw new InvalidOperationException(
                "Khóa học phải có ít nhất một chương hoạt động.");
        }
    }

    private void ValidatePublishable()
    {
        ValidateReadyForReview();

        if (Status != ContentStatus.Approved)
        {
            throw new InvalidOperationException(
                "Khóa học chưa được duyệt.");
        }
    }

    private void EnsureEditable()
    {
        EnsureNotDeleted();

        if (
            Status == ContentStatus.Review ||
            Status == ContentStatus.Approved ||
            Status == ContentStatus.Published ||
            Status == ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "Chỉ khóa học Draft mới được chỉnh sửa.");
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Khóa học đã bị xóa.");
        }
    }

    private void Touch(Guid userId)
    {
        ValidateUserId(
            userId,
            nameof(userId));

        MarkAsUpdated(userId);

        RefreshConcurrencyToken();
    }

    private void RefreshConcurrencyToken()
    {
        ConcurrencyToken =
            Guid.NewGuid();
    }

    private static string NormalizeSlug(
        string value)
    {
        return string.Join(
            '-',
            value.Trim()
                .ToLowerInvariant()
                .Split(
                    [' ', '-', '_'],
                    StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? Normalize(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static void ValidateUserId(
        Guid userId,
        string parameterName)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId không hợp lệ.",
                parameterName);
        }
    }
}