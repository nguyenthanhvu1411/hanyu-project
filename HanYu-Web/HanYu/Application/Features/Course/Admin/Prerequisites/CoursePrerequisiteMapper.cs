using HanYu.Domain.Entities.Course;

namespace HanYu.Application.Features.Course.Admin.Prerequisites;

public static class CoursePrerequisiteMapper
{
    public static CoursePrerequisiteAdminDto ToDto(
        CoursePrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(
            prerequisite);

        return new CoursePrerequisiteAdminDto(
            Id:
                prerequisite.Id,

            PublicId:
                prerequisite.PublicId,

            CourseId:
                prerequisite.CourseId,

            RequiredCourseId:
                prerequisite.RequiredCourseId,

            RequiredCoursePublicId:
                prerequisite.RequiredCourse.PublicId,

            RequiredCourseCode:
                prerequisite.RequiredCourse.Code,

            RequiredCourseSlug:
                prerequisite.RequiredCourse.Slug,

            RequiredCourseTitleVi:
                prerequisite.RequiredCourse.TitleVi,

            IsRequired:
                prerequisite.IsRequired,

            SortOrder:
                prerequisite.SortOrder,

            ConcurrencyToken:
                prerequisite.ConcurrencyToken,

            CreatedAt:
                prerequisite.CreatedAt,

            CreatedById:
                prerequisite.CreatedById,

            UpdatedAt:
                prerequisite.UpdatedAt,

            UpdatedById:
                prerequisite.UpdatedById,

            DeletedAt:
                prerequisite.DeletedAt,

            DeletedById:
                prerequisite.DeletedById);
    }
}
