using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Admin.AudioAssets;
using HanYu.Application.Features.Vocabulary.Admin.Examples;
using HanYu.Application.Features.Vocabulary.Admin.HskLevels;
using HanYu.Application.Features.Vocabulary.Admin.Meanings;
using HanYu.Application.Features.Vocabulary.Admin.PartsOfSpeech;
using HanYu.Application.Features.Vocabulary.Admin.Relations;
using HanYu.Application.Features.Vocabulary.Admin.Topics;
using HanYu.Application.Features.Vocabulary.Admin.Vocabulary;
using HanYu.Application.Features.Vocabulary.Mapping;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Vocabulary;

public sealed class VocabularyAdminService
    : IVocabularyAdminService
{
    private const string GenerationCacheKey =
        "vocabulary:public:generation";

    private readonly HanYuDbContext _db;
    private readonly ICacheService _cache;
    private readonly VocabularyCacheOptions _options;

    public VocabularyAdminService(
        HanYuDbContext db,
        ICacheService cache,
        IOptions<VocabularyCacheOptions> options)
    {
        _db = db;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<Result<
        PagedResult<AdminVocabularyResponse>>>
        GetVocabulariesAsync(
            AdminVocabularyQuery query,
            CancellationToken cancellationToken = default)
    {
        var source =
            _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Include(x => x.HskLevel)
                .Include(x => x.PartOfSpeech)
                .Include(x => x.Topic)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(
                query.Q))
        {
            var keyword =
                query.Q.Trim();

            source =
                source.Where(
                    x =>
                        EF.Functions.ILike(
                            x.Simplified,
                            $"%{keyword}%") ||
                        (
                            x.Traditional != null &&
                            EF.Functions.ILike(
                                x.Traditional,
                                $"%{keyword}%")
                        ) ||
                        EF.Functions.ILike(
                            x.Pinyin,
                            $"%{keyword}%") ||
                        EF.Functions.ILike(
                            x.PrimaryMeaningVi,
                            $"%{keyword}%"));
        }

        if (query.HskLevelId.HasValue)
            source = source.Where(
                x => x.HskLevelId ==
                     query.HskLevelId.Value);

        if (query.PartOfSpeechId.HasValue)
            source = source.Where(
                x => x.PartOfSpeechId ==
                     query.PartOfSpeechId.Value);

        if (query.TopicId.HasValue)
            source = source.Where(
                x => x.TopicId ==
                     query.TopicId.Value);

        if (query.Difficulty.HasValue)
            source = source.Where(
                x => x.Difficulty ==
                     query.Difficulty.Value);

        if (query.Status.HasValue)
            source = source.Where(
                x => x.Status ==
                     query.Status.Value);

        source =
            ApplyAdminSort(
                source,
                query.Sort);

        var total =
            await source.LongCountAsync(
                cancellationToken);

        var entities =
            await source
                .Skip(
                    (query.NormalizedPage - 1) *
                    query.NormalizedPageSize)
                .Take(
                    query.NormalizedPageSize)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success(
            new PagedResult<
                AdminVocabularyResponse>(
                entities
                    .Select(
                        VocabularyMapper
                            .ToAdminResponse)
                    .ToArray(),
                query.NormalizedPage,
                query.NormalizedPageSize,
                total));
    }

    public async Task<Result<AdminVocabularyResponse>>
        GetVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Include(x => x.HskLevel)
                .Include(x => x.PartOfSpeech)
                .Include(x => x.Topic)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                NotFound("Vocabulary"));
        }

        return Result.Success(
            VocabularyMapper
                .ToAdminResponse(entity));
    }

    public async Task<Result<AdminVocabularyResponse>>
        CreateVocabularyAsync(
            CreateVocabularyRequest request,
            CancellationToken cancellationToken = default)
    {
        var references =
            await ValidateReferencesAsync(
                request.HskLevelId,
                request.PartOfSpeechId,
                request.TopicId,
                request.AudioAssetId,
                cancellationToken);

        if (references.IsFailure)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                references.Error);
        }

        var duplicate =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AnyAsync(
                    x =>
                        x.Simplified ==
                            request.Simplified.Trim() &&
                        x.PinyinNormalized ==
                            request.PinyinNormalized.Trim(),
                    cancellationToken);

        if (duplicate)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                Error.Conflict(
                    "Vocabulary.Duplicate",
                    "Vocabulary đã tồn tại."));
        }

        var entity =
            new Domain.Entities.Vocabulary.Vocabulary(
                request.HskLevelId,
                request.Simplified,
                request.Pinyin,
                request.PinyinNormalized,
                request.PrimaryMeaningVi,
                request.Difficulty,
                request.Traditional,
                request.PartOfSpeechId,
                request.TopicId,
                request.AudioAssetId,
                request.NotesVi);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return await GetVocabularyAsync(
            entity.Id,
            cancellationToken);
    }

    public async Task<Result<AdminVocabularyResponse>>
        UpdateVocabularyAsync(
            long id,
            UpdateVocabularyRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                NotFound("Vocabulary"));
        }

        if (entity.Version != request.Version)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                Error.Conflict(
                    "Vocabulary.VersionConflict",
                    "Dữ liệu đã thay đổi. Hãy tải lại."));
        }

        if (entity.Status ==
            ContentStatus.Archived)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                Error.Conflict(
                    "Vocabulary.Archived",
                    "Không thể sửa vocabulary Archived."));
        }

        var references =
            await ValidateReferencesAsync(
                request.HskLevelId,
                request.PartOfSpeechId,
                request.TopicId,
                request.AudioAssetId,
                cancellationToken);

        if (references.IsFailure)
        {
            return Result.Failure<
                AdminVocabularyResponse>(
                references.Error);
        }

        entity.Update(
            request.HskLevelId,
            request.Simplified,
            request.Traditional,
            request.Pinyin,
            request.PinyinNormalized,
            request.PrimaryMeaningVi,
            request.NotesVi,
            request.Difficulty,
            request.PartOfSpeechId,
            request.TopicId,
            request.AudioAssetId);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return await GetVocabularyAsync(
            id,
            cancellationToken);
    }

    public Task<Result>
        SubmitVocabularyForReviewAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        return ChangeVocabularyStatusAsync(
            id,
            x => x.SubmitForReview(),
            cancellationToken);
    }

    public Task<Result>
        ApproveVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        return ChangeVocabularyStatusAsync(
            id,
            x => x.Approve(),
            cancellationToken);
    }

    public Task<Result>
        PublishVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        return ChangeVocabularyStatusAsync(
            id,
            x => x.Publish(),
            cancellationToken);
    }

    public Task<Result>
        ArchiveVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        return ChangeVocabularyStatusAsync(
            id,
            x => x.Archive(),
            cancellationToken);
    }

    public Task<Result>
        RestoreVocabularyAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        return ChangeVocabularyStatusAsync(
            id,
            x => x.Restore(),
            cancellationToken);
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminVocabularyMeaningResponse>>>
        GetMeaningsAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default)
    {
        if (!await VocabularyExistsAsync(
                vocabularyId,
                cancellationToken))
        {
            return Result.Failure<
                IReadOnlyCollection<
                    AdminVocabularyMeaningResponse>>(
                NotFound("Vocabulary"));
        }

        var values =
            await _db.Set<VocabularyMeaning>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.VocabularyId ==
                        vocabularyId)
                .OrderBy(
                    x => x.SenseOrder)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminVocabularyMeaningResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToMeaningResponse)
                .ToArray());
    }

    public async Task<Result<
        AdminVocabularyMeaningResponse>>
        CreateMeaningAsync(
            long vocabularyId,
            CreateVocabularyMeaningRequest request,
            CancellationToken cancellationToken = default)
    {
        if (!await VocabularyExistsAsync(
                vocabularyId,
                cancellationToken))
        {
            return Result.Failure<
                AdminVocabularyMeaningResponse>(
                NotFound("Vocabulary"));
        }

        var orderExists =
            await _db.Set<VocabularyMeaning>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                            vocabularyId &&
                        x.SenseOrder ==
                            request.SenseOrder,
                    cancellationToken);

        if (orderExists)
        {
            return Result.Failure<
                AdminVocabularyMeaningResponse>(
                Error.Conflict(
                    "VocabularyMeaning.DuplicateOrder",
                    "SenseOrder đã tồn tại."));
        }

        var entity =
            new VocabularyMeaning(
                vocabularyId,
                request.MeaningVi,
                request.SenseOrder,
                request.UsageNoteVi);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToMeaningResponse(entity));
    }

    public async Task<Result<
        AdminVocabularyMeaningResponse>>
        UpdateMeaningAsync(
            long vocabularyId,
            long meaningId,
            UpdateVocabularyMeaningRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<VocabularyMeaning>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == meaningId &&
                        x.VocabularyId ==
                            vocabularyId,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure<
                AdminVocabularyMeaningResponse>(
                NotFound("VocabularyMeaning"));
        }

        var duplicate =
            await _db.Set<VocabularyMeaning>()
                .AnyAsync(
                    x =>
                        x.Id != meaningId &&
                        x.VocabularyId ==
                            vocabularyId &&
                        x.SenseOrder ==
                            request.SenseOrder,
                    cancellationToken);

        if (duplicate)
        {
            return Result.Failure<
                AdminVocabularyMeaningResponse>(
                Error.Conflict(
                    "VocabularyMeaning.DuplicateOrder",
                    "SenseOrder đã tồn tại."));
        }

        entity.Update(
            request.MeaningVi,
            request.SenseOrder,
            request.UsageNoteVi);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToMeaningResponse(entity));
    }

    public async Task<Result>
        DeleteMeaningAsync(
            long vocabularyId,
            long meaningId,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<VocabularyMeaning>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == meaningId &&
                        x.VocabularyId ==
                            vocabularyId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("VocabularyMeaning"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminVocabularyExampleResponse>>>
        GetExamplesAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default)
    {
        if (!await VocabularyExistsAsync(
                vocabularyId,
                cancellationToken))
        {
            return Result.Failure<
                IReadOnlyCollection<
                    AdminVocabularyExampleResponse>>(
                NotFound("Vocabulary"));
        }

        var values =
            await _db.Set<VocabularyExample>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.VocabularyId ==
                        vocabularyId)
                .OrderBy(x => x.Id)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminVocabularyExampleResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToExampleResponse)
                .ToArray());
    }

    public async Task<Result<
        AdminVocabularyExampleResponse>>
        CreateExampleAsync(
            long vocabularyId,
            CreateVocabularyExampleRequest request,
            CancellationToken cancellationToken = default)
    {
        if (!await VocabularyExistsAsync(
                vocabularyId,
                cancellationToken))
        {
            return Result.Failure<
                AdminVocabularyExampleResponse>(
                NotFound("Vocabulary"));
        }

        var audioValidation =
            await ValidateAudioAsync(
                request.AudioAssetId,
                cancellationToken);

        if (audioValidation.IsFailure)
        {
            return Result.Failure<
                AdminVocabularyExampleResponse>(
                audioValidation.Error);
        }

        var entity =
            new VocabularyExample(
                vocabularyId,
                request.SentenceZh,
                request.SentencePinyin,
                request.SentenceVi,
                request.Difficulty,
                request.AudioAssetId,
                request.SourceNote);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToExampleResponse(entity));
    }

    public async Task<Result<
        AdminVocabularyExampleResponse>>
        UpdateExampleAsync(
            long vocabularyId,
            long exampleId,
            UpdateVocabularyExampleRequest request,
            CancellationToken cancellationToken = default)
    {
        var result =
            await FindExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<
                AdminVocabularyExampleResponse>(
                result.Error);
        }

        var audio =
            await ValidateAudioAsync(
                request.AudioAssetId,
                cancellationToken);

        if (audio.IsFailure)
        {
            return Result.Failure<
                AdminVocabularyExampleResponse>(
                audio.Error);
        }

        var entity =
            result.Value;

        entity.Update(
            request.SentenceZh,
            request.SentencePinyin,
            request.SentenceVi,
            request.Difficulty,
            request.AudioAssetId,
            request.SourceNote);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToExampleResponse(entity));
    }

    public Task<Result>
        SubmitExampleForReviewAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        return ChangeExampleStatusAsync(
            vocabularyId,
            exampleId,
            x => x.SubmitForReview(),
            cancellationToken);
    }

    public Task<Result>
        ApproveExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        return ChangeExampleStatusAsync(
            vocabularyId,
            exampleId,
            x => x.Approve(),
            cancellationToken);
    }

    public Task<Result>
        PublishExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        return ChangeExampleStatusAsync(
            vocabularyId,
            exampleId,
            x => x.Publish(),
            cancellationToken);
    }

    public Task<Result>
        ArchiveExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        return ChangeExampleStatusAsync(
            vocabularyId,
            exampleId,
            x => x.Archive(),
            cancellationToken);
    }

    public Task<Result>
        RestoreExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        return ChangeExampleStatusAsync(
            vocabularyId,
            exampleId,
            x => x.Restore(),
            cancellationToken);
    }

    public async Task<Result>
        DeleteExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken = default)
    {
        var result =
            await FindExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken);

        if (result.IsFailure)
            return Result.Failure(
                result.Error);

        if (result.Value.Status ==
            ContentStatus.Published)
        {
            return Result.Failure(
                Error.Conflict(
                    "VocabularyExample.Published",
                    "Hãy archive example trước khi xóa."));
        }

        _db.Remove(result.Value);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminVocabularyRelationResponse>>>
        GetRelationsAsync(
            long vocabularyId,
            CancellationToken cancellationToken = default)
    {
        var values =
            await _db.Set<VocabularyRelation>()
                .AsNoTracking()
                .Include(
                    x => x.RelatedVocabulary)
                .Where(
                    x =>
                        x.VocabularyId ==
                        vocabularyId)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminVocabularyRelationResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToRelationResponse)
                .ToArray());
    }

    public async Task<Result<
        AdminVocabularyRelationResponse>>
        CreateRelationAsync(
            long vocabularyId,
            CreateVocabularyRelationRequest request,
            CancellationToken cancellationToken = default)
    {
        if (vocabularyId ==
            request.RelatedVocabularyId)
        {
            return Result.Failure<
                AdminVocabularyRelationResponse>(
                Error.Validation(
                    "VocabularyRelation.SelfReference",
                    "Không thể relation với chính nó."));
        }

        if (!await VocabularyExistsAsync(
                vocabularyId,
                cancellationToken) ||
            !await VocabularyExistsAsync(
                request.RelatedVocabularyId,
                cancellationToken))
        {
            return Result.Failure<
                AdminVocabularyRelationResponse>(
                NotFound("Vocabulary"));
        }

        var duplicate =
            await _db.Set<VocabularyRelation>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                            vocabularyId &&
                        x.RelatedVocabularyId ==
                            request.RelatedVocabularyId &&
                        x.RelationType ==
                            request.RelationType,
                    cancellationToken);

        if (duplicate)
        {
            return Result.Failure<
                AdminVocabularyRelationResponse>(
                Error.Conflict(
                    "VocabularyRelation.Duplicate",
                    "Relation đã tồn tại."));
        }

        var entity =
            new VocabularyRelation(
                vocabularyId,
                request.RelatedVocabularyId,
                request.RelationType,
                request.NoteVi);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        var loaded =
            await _db.Set<VocabularyRelation>()
                .AsNoTracking()
                .Include(
                    x => x.RelatedVocabulary)
                .FirstAsync(
                    x => x.Id == entity.Id,
                    cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToRelationResponse(loaded));
    }

    public async Task<Result<
        AdminVocabularyRelationResponse>>
        UpdateRelationAsync(
            long vocabularyId,
            long relationId,
            UpdateVocabularyRelationRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<VocabularyRelation>()
                .Include(
                    x => x.RelatedVocabulary)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == relationId &&
                        x.VocabularyId ==
                            vocabularyId,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure<
                AdminVocabularyRelationResponse>(
                NotFound("VocabularyRelation"));
        }

        entity.UpdateRelation(
            request.RelationType,
            request.NoteVi);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToRelationResponse(entity));
    }

    public async Task<Result>
        DeleteRelationAsync(
            long vocabularyId,
            long relationId,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<VocabularyRelation>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == relationId &&
                        x.VocabularyId ==
                            vocabularyId,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("VocabularyRelation"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminTopicResponse>>>
        GetTopicsAsync(
            CancellationToken cancellationToken = default)
    {
        var values =
            await _db.Set<Topic>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminTopicResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToTopicResponse)
                .ToArray());
    }

    public async Task<Result<AdminTopicResponse>>
        CreateTopicAsync(
            CreateTopicRequest request,
            CancellationToken cancellationToken = default)
    {
        var slug =
            request.Slug.Trim()
                .ToLowerInvariant();

        if (await _db.Set<Topic>()
                .AnyAsync(
                    x => x.Slug == slug,
                    cancellationToken))
        {
            return Result.Failure<
                AdminTopicResponse>(
                Error.Conflict(
                    "Topic.DuplicateSlug",
                    "Slug đã tồn tại."));
        }

        var entity =
            new Topic(
                request.Slug,
                request.NameVi,
                request.DescriptionVi,
                request.SortOrder);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToTopicResponse(entity));
    }

    public async Task<Result<AdminTopicResponse>>
        UpdateTopicAsync(
            long id,
            UpdateTopicRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<Topic>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<
                AdminTopicResponse>(
                NotFound("Topic"));

        entity.Update(
            request.Slug,
            request.NameVi,
            request.DescriptionVi,
            request.SortOrder);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToTopicResponse(entity));
    }

    public Task<Result> PublishTopicAsync(
        long id,
        CancellationToken cancellationToken = default)
        => ChangeTopicStatusAsync(
            id,
            x => x.Publish(),
            cancellationToken);

    public Task<Result> ArchiveTopicAsync(
        long id,
        CancellationToken cancellationToken = default)
        => ChangeTopicStatusAsync(
            id,
            x => x.Archive(),
            cancellationToken);

    public Task<Result> RestoreTopicAsync(
        long id,
        CancellationToken cancellationToken = default)
        => ChangeTopicStatusAsync(
            id,
            x => x.Restore(),
            cancellationToken);

    public async Task<Result>
        DeleteTopicAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        if (await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AnyAsync(
                    x => x.TopicId == id,
                    cancellationToken))
        {
            return Result.Failure(
                Error.Conflict(
                    "Topic.InUse",
                    "Topic đang được sử dụng."));
        }

        var entity =
            await _db.Set<Topic>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("Topic"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminPartOfSpeechResponse>>>
        GetPartsOfSpeechAsync(
            CancellationToken cancellationToken = default)
    {
        var values =
            await _db.Set<PartOfSpeech>()
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminPartOfSpeechResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToPartOfSpeechResponse)
                .ToArray());
    }

    public async Task<Result<
        AdminPartOfSpeechResponse>>
        CreatePartOfSpeechAsync(
            CreatePartOfSpeechRequest request,
            CancellationToken cancellationToken = default)
    {
        var code =
            request.Code.Trim()
                .ToLowerInvariant();

        if (await _db.Set<PartOfSpeech>()
                .AnyAsync(
                    x => x.Code == code,
                    cancellationToken))
        {
            return Result.Failure<
                AdminPartOfSpeechResponse>(
                Error.Conflict(
                    "PartOfSpeech.Duplicate",
                    "Code đã tồn tại."));
        }

        var entity =
            new PartOfSpeech(
                request.Code,
                request.NameVi,
                request.NameEn);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToPartOfSpeechResponse(entity));
    }

    public async Task<Result<
        AdminPartOfSpeechResponse>>
        UpdatePartOfSpeechAsync(
            long id,
            UpdatePartOfSpeechRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<PartOfSpeech>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<
                AdminPartOfSpeechResponse>(
                NotFound("PartOfSpeech"));

        entity.Update(
            request.Code,
            request.NameVi,
            request.NameEn);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToPartOfSpeechResponse(entity));
    }

    public async Task<Result>
        DeletePartOfSpeechAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        if (await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AnyAsync(
                    x =>
                        x.PartOfSpeechId == id,
                    cancellationToken))
        {
            return Result.Failure(
                Error.Conflict(
                    "PartOfSpeech.InUse",
                    "PartOfSpeech đang được sử dụng."));
        }

        var entity =
            await _db.Set<PartOfSpeech>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("PartOfSpeech"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<
            AdminHskLevelResponse>>>
        GetHskLevelsAsync(
            CancellationToken cancellationToken = default)
    {
        var values =
            await _db.Set<HskLevel>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                AdminHskLevelResponse>>(
            values
                .Select(
                    VocabularyAdminMapper
                        .ToHskResponse)
                .ToArray());
    }

    public async Task<Result<
        AdminHskLevelResponse>>
        CreateHskLevelAsync(
            CreateHskLevelRequest request,
            CancellationToken cancellationToken = default)
    {
        if (await _db.Set<HskLevel>()
                .AnyAsync(
                    x => x.Code == request.Code,
                    cancellationToken))
        {
            return Result.Failure<
                AdminHskLevelResponse>(
                Error.Conflict(
                    "HskLevel.Duplicate",
                    "HSK level đã tồn tại."));
        }

        var entity =
            new HskLevel(
                request.Code,
                request.NameVi,
                request.SortOrder);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToHskResponse(entity));
    }

    public async Task<Result<
        AdminHskLevelResponse>>
        UpdateHskLevelAsync(
            long id,
            UpdateHskLevelRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<HskLevel>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<
                AdminHskLevelResponse>(
                NotFound("HskLevel"));

        entity.Update(
            request.Code,
            request.NameVi,
            request.SortOrder,
            Guid.Empty);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToHskResponse(entity));
    }

    public Task<Result> ActivateHskLevelAsync(
        long id,
        CancellationToken cancellationToken = default)
        => ChangeHskStateAsync(
            id,
            x => x.Activate(Guid.Empty),
            cancellationToken);

    public Task<Result> DeactivateHskLevelAsync(
        long id,
        CancellationToken cancellationToken = default)
        => ChangeHskStateAsync(
            id,
            x => x.Deactivate(Guid.Empty),
            cancellationToken);

    public async Task<Result>
        DeleteHskLevelAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        if (await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AnyAsync(
                    x =>
                        x.HskLevelId == id,
                    cancellationToken))
        {
            return Result.Failure(
                Error.Conflict(
                    "HskLevel.InUse",
                    "HSK level đang được sử dụng."));
        }

        var entity =
            await _db.Set<HskLevel>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("HskLevel"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        PagedResult<AdminAudioAssetResponse>>>
        GetAudioAssetsAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(
            pageSize,
            1,
            100);

        var source =
            _db.Set<AudioAsset>()
                .AsNoTracking()
                .OrderByDescending(
                    x => x.UpdatedAt);

        var total =
            await source.LongCountAsync(
                cancellationToken);

        var values =
            await source
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success(
            new PagedResult<
                AdminAudioAssetResponse>(
                values
                    .Select(
                        VocabularyAdminMapper
                            .ToAudioResponse)
                .ToArray(),
                page,
                pageSize,
                total));
    }

    public async Task<Result<
        AdminAudioAssetResponse>>
        CreateAudioAssetAsync(
            CreateAudioAssetRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            new AudioAsset(
                request.StoragePath,
                request.Kind,
                request.MimeType);

        _db.Add(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToAudioResponse(entity));
    }

    public async Task<Result<
        AdminAudioAssetResponse>>
        UpdateAudioAssetAsync(
            long id,
            UpdateAudioAssetRequest request,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await _db.Set<AudioAsset>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure<
                AdminAudioAssetResponse>(
                NotFound("AudioAsset"));

        entity.UpdateFileInfo(
            request.StoragePath,
            request.MimeType,
            request.FileSizeBytes,
            request.DurationMs,
            request.Checksum);

        entity.UpdateAudioMetadata(
            request.Voice,
            request.Provider,
            request.LanguageCode);

        entity.UpdatePublicUrl(
            request.PublicUrl);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success(
            VocabularyAdminMapper
                .ToAudioResponse(entity));
    }

    public async Task<Result>
        PublishAudioAssetAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await FindAudioAsync(
                id,
                cancellationToken);

        if (entity.IsFailure)
            return Result.Failure(
                entity.Error);

        entity.Value.Publish();

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result>
        ArchiveAudioAssetAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var entity =
            await FindAudioAsync(
                id,
                cancellationToken);

        if (entity.IsFailure)
            return Result.Failure(
                entity.Error);

        entity.Value.Archive();

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result>
        DeleteAudioAssetAsync(
            long id,
            CancellationToken cancellationToken = default)
    {
        var inVocabulary =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AnyAsync(
                    x => x.AudioAssetId == id,
                    cancellationToken);

        var inExample =
            await _db.Set<VocabularyExample>()
                .AnyAsync(
                    x => x.AudioAssetId == id,
                    cancellationToken);

        if (inVocabulary || inExample)
        {
            return Result.Failure(
                Error.Conflict(
                    "AudioAsset.InUse",
                    "AudioAsset đang được sử dụng."));
        }

        var entity =
            await _db.Set<AudioAsset>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("AudioAsset"));

        _db.Remove(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result>
        ChangeVocabularyStatusAsync(
            long id,
            Action<
                Domain.Entities.Vocabulary.Vocabulary> action,
            CancellationToken cancellationToken)
    {
        var entity =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("Vocabulary"));

        action(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result>
        ChangeExampleStatusAsync(
            long vocabularyId,
            long exampleId,
            Action<VocabularyExample> action,
            CancellationToken cancellationToken)
    {
        var result =
            await FindExampleAsync(
                vocabularyId,
                exampleId,
                cancellationToken);

        if (result.IsFailure)
            return Result.Failure(
                result.Error);

        action(result.Value);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result>
        ChangeTopicStatusAsync(
            long id,
            Action<Topic> action,
            CancellationToken cancellationToken)
    {
        var entity =
            await _db.Set<Topic>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("Topic"));

        action(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result>
        ChangeHskStateAsync(
            long id,
            Action<HskLevel> action,
            CancellationToken cancellationToken)
    {
        var entity =
            await _db.Set<HskLevel>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
            return Result.Failure(
                NotFound("HskLevel"));

        action(entity);

        await _db.SaveChangesAsync(
            cancellationToken);

        await BumpCacheGenerationAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result<VocabularyExample>>
        FindExampleAsync(
            long vocabularyId,
            long exampleId,
            CancellationToken cancellationToken)
    {
        var entity =
            await _db.Set<VocabularyExample>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == exampleId &&
                        x.VocabularyId ==
                            vocabularyId,
                    cancellationToken);

        return entity is null
            ? Result.Failure<VocabularyExample>(
                NotFound("VocabularyExample"))
            : Result.Success(entity);
    }

    private async Task<Result<AudioAsset>>
        FindAudioAsync(
            long id,
            CancellationToken cancellationToken)
    {
        var entity =
            await _db.Set<AudioAsset>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        return entity is null
            ? Result.Failure<AudioAsset>(
                NotFound("AudioAsset"))
            : Result.Success(entity);
    }

    private Task<bool> VocabularyExistsAsync(
        long id,
        CancellationToken cancellationToken)
    {
        return _db.Set<
                Domain.Entities.Vocabulary.Vocabulary>()
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);
    }

    private async Task<Result>
        ValidateReferencesAsync(
            long hskLevelId,
            long? partOfSpeechId,
            long? topicId,
            long? audioAssetId,
            CancellationToken cancellationToken)
    {
        var hsk =
            await _db.Set<HskLevel>()
                .AnyAsync(
                    x =>
                        x.Id == hskLevelId &&
                        x.IsActive,
                    cancellationToken);

        if (!hsk)
            return Result.Failure(
                Error.Validation(
                    "Vocabulary.InvalidHsk",
                    "HSK không hợp lệ."));

        if (partOfSpeechId.HasValue &&
            !await _db.Set<PartOfSpeech>()
                .AnyAsync(
                    x =>
                        x.Id ==
                        partOfSpeechId.Value,
                    cancellationToken))
        {
            return Result.Failure(
                Error.Validation(
                    "Vocabulary.InvalidPartOfSpeech",
                    "PartOfSpeech không tồn tại."));
        }

        if (topicId.HasValue &&
            !await _db.Set<Topic>()
                .AnyAsync(
                    x =>
                        x.Id ==
                        topicId.Value,
                    cancellationToken))
        {
            return Result.Failure(
                Error.Validation(
                    "Vocabulary.InvalidTopic",
                    "Topic không tồn tại."));
        }

        return await ValidateAudioAsync(
            audioAssetId,
            cancellationToken);
    }

    private async Task<Result>
        ValidateAudioAsync(
            long? audioAssetId,
            CancellationToken cancellationToken)
    {
        if (!audioAssetId.HasValue)
            return Result.Success();

        var audio =
            await _db.Set<AudioAsset>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                        audioAssetId.Value,
                    cancellationToken);

        if (audio is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AudioAsset.NotFound",
                    "AudioAsset không tồn tại."));
        }

        if (audio.Status ==
            ContentStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict(
                    "AudioAsset.Archived",
                    "AudioAsset đã Archived."));
        }

        return Result.Success();
    }

    private async Task
        BumpCacheGenerationAsync(
            CancellationToken cancellationToken)
    {
        await _cache.SetAsync(
            GenerationCacheKey,
            Guid.NewGuid().ToString("N"),
            TimeSpan.FromDays(
                _options.GenerationDays),
            cancellationToken);
    }

    private static Error NotFound(
        string resource)
    {
        return Error.NotFound(
            $"{resource}.NotFound",
            $"Không tìm thấy {resource}.");
    }

    private static IQueryable<
        Domain.Entities.Vocabulary.Vocabulary>
        ApplyAdminSort(
            IQueryable<
                Domain.Entities.Vocabulary.Vocabulary> source,
            string? sort)
    {
        return sort?
            .Trim()
            .ToLowerInvariant()
            switch
        {
            "simplified" =>
                source.OrderBy(
                    x => x.Simplified),

            "-simplified" =>
                source.OrderByDescending(
                    x => x.Simplified),

            "createdat" =>
                source.OrderBy(
                    x => x.CreatedAt),

            "-createdat" =>
                source.OrderByDescending(
                    x => x.CreatedAt),

            "updatedat" =>
                source.OrderBy(
                    x => x.UpdatedAt),

            "-updatedat" =>
                source.OrderByDescending(
                    x => x.UpdatedAt),

            _ =>
                source.OrderByDescending(
                    x => x.UpdatedAt)
        };
    }
    public async Task<Result> DeleteVocabularyAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result.Failure(
                Error.Validation(
                    "Vocabulary.InvalidId",
                    "Vocabulary ID không hợp lệ."));
        }

        var entity =
            await _db
                .Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

        if (entity is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy vocabulary."));
        }

        if (entity.Status != ContentStatus.Draft &&
            entity.Status != ContentStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.DeleteInvalidStatus",
                    "Chỉ được xóa vocabulary ở trạng thái Draft hoặc Archived."));
        }

        var dependencyResult =
            await CheckVocabularyDeleteDependenciesAsync(
                id,
                cancellationToken);

        if (dependencyResult.IsFailure)
        {
            return dependencyResult;
        }

        await using var transaction =
            await _db.Database
                .BeginTransactionAsync(
                    cancellationToken);

        try
        {
            var notes =
                await _db
                    .Set<UserVocabularyNote>()
                    .Where(
                        x => x.VocabularyId == id)
                    .ToListAsync(
                        cancellationToken);

            if (notes.Count > 0)
            {
                _db.Set<UserVocabularyNote>()
                    .RemoveRange(notes);
            }

            var meanings =
                await _db
                    .Set<VocabularyMeaning>()
                    .Where(
                        x => x.VocabularyId == id)
                    .ToListAsync(
                        cancellationToken);

            if (meanings.Count > 0)
            {
                _db.Set<VocabularyMeaning>()
                    .RemoveRange(meanings);
            }

            var examples =
                await _db
                    .Set<VocabularyExample>()
                    .Where(
                        x => x.VocabularyId == id)
                    .ToListAsync(
                        cancellationToken);

            if (examples.Count > 0)
            {
                _db.Set<VocabularyExample>()
                    .RemoveRange(examples);
            }

            var relations =
                await _db
                    .Set<VocabularyRelation>()
                    .Where(
                        x =>
                            x.VocabularyId == id ||
                            x.RelatedVocabularyId == id)
                    .ToListAsync(
                        cancellationToken);

            if (relations.Count > 0)
            {
                _db.Set<VocabularyRelation>()
                    .RemoveRange(relations);
            }

            _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .Remove(entity);

            await _db.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            await BumpCacheGenerationAsync(
                cancellationToken);

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    private async Task<Result>
        CheckVocabularyDeleteDependenciesAsync(
            long vocabularyId,
            CancellationToken cancellationToken)
    {
        var usedByLesson =
            await _db
                .Set<LessonVocabulary>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (usedByLesson)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.InUseByLesson",
                    "Vocabulary đang được sử dụng trong Lesson. Hãy Archive thay vì xóa."));
        }

        var usedByQuiz =
            await _db
                .Set<QuizQuestion>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (usedByQuiz)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.InUseByQuiz",
                    "Vocabulary đang được sử dụng trong Quiz. Hãy Archive thay vì xóa."));
        }

        var usedByLearningActivity =
            await _db
                .Set<LearningActivity>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (usedByLearningActivity)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.HasLearningHistory",
                    "Vocabulary đã có lịch sử học và không thể xóa. Hãy Archive."));
        }

        var hasVocabularyState =
            await _db
                .Set<UserVocabularyState>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (hasVocabularyState)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.HasLearningState",
                    "Vocabulary đã có dữ liệu tiến độ/SRS của người học và không thể xóa."));
        }

        var hasReviewEvents =
            await _db
                .Set<ReviewEvent>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (hasReviewEvents)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.HasReviewHistory",
                    "Vocabulary đã có lịch sử ôn tập và không thể xóa."));
        }

        var usedByFlashcard =
            await _db
                .Set<FlashcardSessionItem>()
                .AnyAsync(
                    x =>
                        x.VocabularyId ==
                        vocabularyId,
                    cancellationToken);

        if (usedByFlashcard)
        {
            return Result.Failure(
                Error.Conflict(
                    "Vocabulary.HasFlashcardHistory",
                    "Vocabulary đã được sử dụng trong Flashcard Session và không thể xóa."));
        }

        return Result.Success();
    }
}
