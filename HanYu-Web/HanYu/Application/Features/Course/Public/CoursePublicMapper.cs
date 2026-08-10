using HanYu.Domain.Entities.Course;

using CourseEntity =
    HanYu.Domain.Entities.Course.Course;

namespace HanYu.Application.Features.Course.Public;

public static class CoursePublicMapper
{
    public static PublicCourseListItemDto ToListItemDto(
        CourseEntity course)
    {
        ArgumentNullException.ThrowIfNull(
            course);

        return new PublicCourseListItemDto(
            PublicId:
                course.PublicId,

            Slug:
                course.Slug,

            TitleVi:
                course.TitleVi,

            ShortDescriptionVi:
                course.ShortDescriptionVi,

            HskCode:
                course.HskLevel?.Code,

            HskNameVi:
                course.HskLevel?.NameVi,

            CoverImageUrl:
                course.CoverImageUrl,

            EstimatedMinutes:
                course.EstimatedMinutes,

            IsFeatured:
                course.IsFeatured,

            ChapterCount:
                course.Chapters.Count(
                    x =>
                        !x.IsDeleted &&
                        x.IsActive));
    }

    public static PublicCourseDetailDto ToDetailDto(
        CourseEntity course)
    {
        ArgumentNullException.ThrowIfNull(
            course);

        return new PublicCourseDetailDto(
            PublicId:
                course.PublicId,

            Slug:
                course.Slug,

            TitleVi:
                course.TitleVi,

            ShortDescriptionVi:
                course.ShortDescriptionVi,

            DescriptionVi:
                course.DescriptionVi,

            HskCode:
                course.HskLevel?.Code,

            HskNameVi:
                course.HskLevel?.NameVi,

            CoverImageUrl:
                course.CoverImageUrl,

            EstimatedMinutes:
                course.EstimatedMinutes,

            IsFeatured:
                course.IsFeatured,

            Chapters:
                course.Chapters
                    .Where(
                        x =>
                            !x.IsDeleted &&
                            x.IsActive)
                    .OrderBy(
                        x => x.SortOrder)
                    .Select(
                        ToChapterDto)
                    .ToList(),

            Prerequisites:
                course.Prerequisites
                    .Where(
                        x =>
                            !x.IsDeleted &&
                            x.RequiredCourse.IsActive)
                    .OrderBy(
                        x => x.SortOrder)
                    .Select(
                        ToPrerequisiteDto)
                    .ToList());
    }

    private static PublicCourseChapterDto ToChapterDto(
        CourseChapter chapter)
    {
        return new PublicCourseChapterDto(
            PublicId:
                chapter.PublicId,

            TitleVi:
                chapter.TitleVi,

            DescriptionVi:
                chapter.DescriptionVi,

            SortOrder:
                chapter.SortOrder,

            LessonCount:
                0,

            Lessons:
                []);
    }

    private static PublicCoursePrerequisiteDto ToPrerequisiteDto(
        CoursePrerequisite prerequisite)
    {
        return new PublicCoursePrerequisiteDto(
            PublicId:
                prerequisite.RequiredCourse.PublicId,

            Slug:
                prerequisite.RequiredCourse.Slug,

            TitleVi:
                prerequisite.RequiredCourse.TitleVi,

            IsRequired:
                prerequisite.IsRequired);
    }
}
