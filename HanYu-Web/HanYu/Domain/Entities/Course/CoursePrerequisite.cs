using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Course;

public sealed class CoursePrerequisite : AuditableEntity
{
    public long CourseId { get; private set; }

    public long RequiredCourseId { get; private set; }

    public bool IsRequired { get; private set; }
        = true;

    public int SortOrder { get; private set; }

    public Guid ConcurrencyToken { get; private set; }
        = Guid.NewGuid();

    public Course Course { get; private set; }
        = null!;

    public Course RequiredCourse { get; private set; }
        = null!;

    private CoursePrerequisite()
    {
    }

    public CoursePrerequisite(
        long courseId,
        long requiredCourseId,
        int sortOrder = 0,
        bool isRequired = true)
    {
        ValidateCourseIds(
            courseId,
            requiredCourseId);

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));
        }

        CourseId = courseId;
        RequiredCourseId = requiredCourseId;
        SortOrder = sortOrder;
        IsRequired = isRequired;
    }

    public void Update(
        long requiredCourseId,
        bool isRequired,
        int sortOrder,
        Guid updatedById)
    {
        EnsureNotDeleted();

        ValidateCourseIds(CourseId, requiredCourseId);

        RequiredCourseId = requiredCourseId;
        IsRequired = isRequired;
        
        SetSortOrder(sortOrder);

        Touch(updatedById);
    }

    public void ChangeSortOrder(
        int sortOrder,
        Guid updatedById)
    {
        EnsureNotDeleted();

        SetSortOrder(sortOrder);

        Touch(updatedById);
    }

    public void Delete(
        Guid deletedById)
    {
        EnsureUserId(deletedById, nameof(deletedById));

        if (IsDeleted)
            return;

        SoftDelete(deletedById);

        ConcurrencyToken = Guid.NewGuid();
    }

    public void RestoreDeleted(
        Guid restoredById)
    {
        EnsureUserId(restoredById, nameof(restoredById));

        if (!IsDeleted)
            return;

        Restore(restoredById);

        ConcurrencyToken = Guid.NewGuid();
    }

    private void SetSortOrder(
        int sortOrder)
    {
        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sortOrder));
        }

        SortOrder = sortOrder;
    }

    private static void ValidateCourseIds(
        long courseId,
        long requiredCourseId)
    {
        if (courseId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(courseId));
        }

        if (requiredCourseId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredCourseId));
        }

        if (courseId == requiredCourseId)
        {
            throw new InvalidOperationException(
                "Khóa học không thể là prerequisite của chính nó.");
        }
    }

    private void Touch(
        Guid updatedById)
    {
        EnsureUserId(updatedById, nameof(updatedById));

        MarkAsUpdated(updatedById);

        ConcurrencyToken = Guid.NewGuid();
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Course prerequisite đã bị xóa.");
        }
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