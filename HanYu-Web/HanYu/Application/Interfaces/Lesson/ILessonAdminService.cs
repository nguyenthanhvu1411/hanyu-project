using HanYu.Application.Common.Models;
using HanYu.Application.Features.Lesson.Admin.Assets;
using HanYu.Application.Features.Lesson.Admin.Lessons;
using HanYu.Application.Features.Lesson.Admin.Prerequisites;
using HanYu.Application.Features.Lesson.Admin.Sections;
using HanYu.Application.Features.Lesson.Admin.Vocabulary;

namespace HanYu.Application.Interfaces.Lesson;

public interface ILessonAdminService
{
    Task<Result<PagedResult<AdminLessonListItemDto>>> GetLessonsAsync(
        AdminLessonQuery query,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> GetLessonAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<HanYu.Application.Features.Lesson.Admin.Lessons.LessonValidationResultDto>> ValidateLessonAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> CreateLessonAsync(
        CreateLessonRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> UpdateLessonAsync(
        long id,
        UpdateLessonRequest request,
        CancellationToken cancellationToken = default);


    Task<Result<AdminLessonDetailDto>> SubmitForReviewAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> ApproveAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> PublishAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> ArchiveAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> RestoreAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonDetailDto>> RestoreDeletedAsync(
        long id,
        LessonWorkflowRequest request,
        CancellationToken cancellationToken = default);

    // Section APIs
    Task<Result<IReadOnlyCollection<AdminLessonSectionResponse>>> GetSectionsAsync(
        long lessonId,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonSectionResponse>> CreateSectionAsync(
        long lessonId,
        CreateLessonSectionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonSectionResponse>> UpdateSectionAsync(
        long lessonId,
        long sectionId,
        UpdateLessonSectionRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteSectionAsync(
        long lessonId,
        long sectionId,
        CancellationToken cancellationToken = default);

    // Asset APIs
    Task<Result<IReadOnlyCollection<AdminLessonAssetResponse>>> GetAssetsAsync(
        long lessonId,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonAssetResponse>> CreateAssetAsync(
        long lessonId,
        CreateLessonAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonAssetResponse>> UpdateAssetAsync(
        long lessonId,
        long assetId,
        UpdateLessonAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAssetAsync(
        long lessonId,
        long assetId,
        CancellationToken cancellationToken = default);

    // Vocabulary APIs
    Task<Result<IReadOnlyCollection<AdminLessonVocabularyResponse>>> GetVocabularyAsync(
        long lessonId,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonVocabularyResponse>> AttachVocabularyAsync(
        long lessonId,
        AttachLessonVocabularyRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonVocabularyResponse>> UpdateVocabularyAsync(
        long lessonId,
        long vocabularyId,
        UpdateLessonVocabularyRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> DetachVocabularyAsync(
        long lessonId,
        long vocabularyId,
        CancellationToken cancellationToken = default);

    // Prerequisite APIs
    Task<Result<IReadOnlyCollection<AdminLessonPrerequisiteResponse>>> GetPrerequisitesAsync(
        long lessonId,
        CancellationToken cancellationToken = default);

    Task<Result<AdminLessonPrerequisiteResponse>> AddPrerequisiteAsync(
        long lessonId,
        AddLessonPrerequisiteRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> RemovePrerequisiteAsync(
        long lessonId,
        long requiredLessonId,
        CancellationToken cancellationToken = default);
}
