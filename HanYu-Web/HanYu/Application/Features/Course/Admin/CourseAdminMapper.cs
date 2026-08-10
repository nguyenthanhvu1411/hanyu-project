using HanYu.Domain.Entities.Course;

using CourseEntity =
    HanYu.Domain.Entities.Course.Course;

namespace HanYu.Application.Features.Course.Admin;

public static class CourseAdminMapper
{
    public static AdminCourseListItemDto ToListItemDto(
        CourseEntity course)
    {
        ArgumentNullException.ThrowIfNull(
            course);

        return new AdminCourseListItemDto(
            Id: course.Id,
            PublicId: course.PublicId,
            Code: course.Code,
            Slug: course.Slug,
            TitleVi: course.TitleVi,

            HskLevelId:
                course.HskLevelId,

            HskCode:
                course.HskLevel?.Code,

            HskNameVi:
                course.HskLevel?.NameVi,

            CoverImageUrl:
                course.CoverImageUrl,

            SortOrder:
                course.SortOrder,

            EstimatedMinutes:
                course.EstimatedMinutes,

            Status:
                course.Status,

            IsActive:
                course.IsActive,

            IsFeatured:
                course.IsFeatured,

            ChapterCount:
                course.Chapters.Count(
                    x => !x.IsDeleted),

            PublishedAt:
                course.PublishedAt,

            CreatedAt:
                course.CreatedAt,

            UpdatedAt:
                course.UpdatedAt);
    }

    public static AdminCourseDetailDto ToDetailDto(
        CourseEntity course)
    {
        ArgumentNullException.ThrowIfNull(
            course);

        return new AdminCourseDetailDto(
            Id:
                course.Id,

            PublicId:
                course.PublicId,

            Code:
                course.Code,

            Slug:
                course.Slug,

            TitleVi:
                course.TitleVi,

            ShortDescriptionVi:
                course.ShortDescriptionVi,

            DescriptionVi:
                course.DescriptionVi,

            HskLevelId:
                course.HskLevelId,

            HskCode:
                course.HskLevel?.Code,

            HskNameVi:
                course.HskLevel?.NameVi,

            CoverImageUrl:
                course.CoverImageUrl,

            SortOrder:
                course.SortOrder,

            EstimatedMinutes:
                course.EstimatedMinutes,

            Status:
                course.Status,

            IsActive:
                course.IsActive,

            IsFeatured:
                course.IsFeatured,

            PublishedAt:
                course.PublishedAt,

            PublishedById:
                course.PublishedById,

            ArchivedAt:
                course.ArchivedAt,

            ArchivedById:
                course.ArchivedById,

            ConcurrencyToken:
                course.ConcurrencyToken,

            CreatedAt:
                course.CreatedAt,

            CreatedById:
                course.CreatedById,

            UpdatedAt:
                course.UpdatedAt,

            UpdatedById:
                course.UpdatedById,

            DeletedAt:
                course.DeletedAt,

            DeletedById:
                course.DeletedById,

            Chapters:
                course.Chapters
                    .Where(
                        x => !x.IsDeleted)
                    .OrderBy(
                        x => x.SortOrder)
                    .Select(
                        ToChapterDto)
                    .ToList(),

            Prerequisites:
                course.Prerequisites
                    .Where(
                        x => !x.IsDeleted)
                    .OrderBy(
                        x => x.SortOrder)
                    .Select(
                        ToPrerequisiteDto)
                    .ToList());
    }

    public static AdminCourseChapterDto ToChapterDto(
        CourseChapter chapter)
    {
        ArgumentNullException.ThrowIfNull(
            chapter);

        return new AdminCourseChapterDto(
            Id:
                chapter.Id,

            PublicId:
                chapter.PublicId,

            CourseId:
                chapter.CourseId,

            TitleVi:
                chapter.TitleVi,

            DescriptionVi:
                chapter.DescriptionVi,

            SortOrder:
                chapter.SortOrder,

            IsActive:
                chapter.IsActive,

            LessonCount:
                chapter.Lessons.Count,

            ConcurrencyToken:
                chapter.ConcurrencyToken,

            CreatedAt:
                chapter.CreatedAt,

            UpdatedAt:
                chapter.UpdatedAt,

            DeletedAt:
                chapter.DeletedAt,

            DeletedById:
                chapter.DeletedById);
    }

    public static AdminCoursePrerequisiteDto ToPrerequisiteDto(
        CoursePrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(
            prerequisite);

        return new AdminCoursePrerequisiteDto(
            Id:
                prerequisite.Id,

            RequiredCourseId:
                prerequisite.RequiredCourseId,

            RequiredCoursePublicId:
                prerequisite.RequiredCourse.PublicId,

            RequiredCourseCode:
                prerequisite.RequiredCourse.Code,

            RequiredCourseTitleVi:
                prerequisite.RequiredCourse.TitleVi,

            IsRequired:
                prerequisite.IsRequired,

            SortOrder:
                prerequisite.SortOrder);
    }
}
