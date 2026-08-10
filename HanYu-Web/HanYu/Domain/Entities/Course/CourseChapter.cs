using HanYu.Domain.Entities;
using LessonEntity = HanYu.Domain.Entities.Lesson.Lesson;

namespace HanYu.Domain.Entities.Course;

public sealed class CourseChapter : AuditableEntity
{
    public long CourseId { get; private set; }

    public string TitleVi { get; private set; }
        = string.Empty;

    public string? DescriptionVi { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }
        = true;

    public Guid ConcurrencyToken { get; private set; }
        = Guid.NewGuid();

    public Course Course { get; private set; }
        = null!;

    public ICollection<LessonEntity> Lessons { get; private set; }
        = new List<LessonEntity>();

    private CourseChapter()
    {
    }

    public CourseChapter(
        long courseId,
        string titleVi,
        int sortOrder,
        string? descriptionVi = null,
        bool isActive = true)
    {
        if (courseId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(courseId));
        }

        CourseId = courseId;

        SetTitleVi(titleVi);
        SetDescriptionVi(descriptionVi);
        SetSortOrder(sortOrder);
        IsActive = isActive;
    }

    public void Update(
        string titleVi,
        string? descriptionVi,
        int sortOrder,
        bool isActive,
        Guid updatedById)
    {
        EnsureNotDeleted();

        SetTitleVi(titleVi);
        SetDescriptionVi(descriptionVi);
        SetSortOrder(sortOrder);
        IsActive = isActive;

        RotateConcurrencyToken();
        Touch(updatedById);
    }

    public void ChangeSortOrder(
        int sortOrder,
        Guid updatedById)
    {
        EnsureNotDeleted();

        SetSortOrder(sortOrder);

        RotateConcurrencyToken();
        Touch(updatedById);
    }

    public void Activate(
        Guid updatedById)
    {
        EnsureNotDeleted();

        if (IsActive)
            return;

        IsActive = true;

        RotateConcurrencyToken();
        Touch(updatedById);
    }

    public void Deactivate(
        Guid updatedById)
    {
        EnsureNotDeleted();

        if (!IsActive)
            return;

        IsActive = false;

        RotateConcurrencyToken();
        Touch(updatedById);
    }

    public void Delete(
        Guid deletedById)
    {
        EnsureUserId(deletedById, nameof(deletedById));

        if (IsDeleted)
            return;

        SoftDelete(deletedById);
        IsActive = false;
        RotateConcurrencyToken();
    }

    public new void Restore(
        Guid restoredById)
    {
        EnsureUserId(restoredById, nameof(restoredById));

        if (!IsDeleted)
            return;

        base.Restore(restoredById);
        IsActive = true;
        RotateConcurrencyToken();
    }

    public void RestoreDeleted(
        Guid restoredById)
        => Restore(restoredById);

    private void RotateConcurrencyToken()
        => ConcurrencyToken = Guid.NewGuid();

    private void SetTitleVi(
        string titleVi)
    {
        if (string.IsNullOrWhiteSpace(titleVi))
        {
            throw new ArgumentException(
                "Tên chương không được để trống.",
                nameof(titleVi));
        }

        titleVi = titleVi.Trim();

        if (titleVi.Length > 200)
        {
            throw new ArgumentException(
                "Tên chương không được vượt quá 200 ký tự.",
                nameof(titleVi));
        }

        TitleVi = titleVi;
    }

    private void SetDescriptionVi(
        string? descriptionVi)
    {
        DescriptionVi = string.IsNullOrWhiteSpace(descriptionVi)
            ? null
            : descriptionVi.Trim();
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

    private static void EnsureUserId(
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
