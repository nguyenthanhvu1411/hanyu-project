using System.Security.Cryptography;
using System.Text;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Vocabulary.Mapping;
using HanYu.Application.Features.Vocabulary.Public.Notes;
using HanYu.Application.Features.Vocabulary.Public.Taxonomy;
using HanYu.Application.Features.Vocabulary.Public.Vocabulary;
using HanYu.Application.Interfaces.Caching;
using HanYu.Application.Interfaces.Vocabulary;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Vocabulary;

public sealed class VocabularyPublicService
    : IVocabularyPublicService
{
    private const string GenerationCacheKey =
        "vocabulary:public:generation";

    private readonly HanYuDbContext _db;
    private readonly ICacheService _cache;
    private readonly VocabularyCacheOptions _options;

    public VocabularyPublicService(
        HanYuDbContext db,
        ICacheService cache,
        IOptions<VocabularyCacheOptions> options)
    {
        _db = db;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<Result<
        PagedResult<VocabularyListItemResponse>>>
        GetVocabulariesAsync(
            VocabularyQuery query,
            CancellationToken cancellationToken = default)
    {
        var generation =
            await GetGenerationAsync(
                cancellationToken);

        var rawCacheKey =
            string.Join(
                "|",
                query.Q,
                query.HskLevel,
                query.Topic,
                query.PartOfSpeech,
                query.Difficulty,
                query.Sort,
                query.NormalizedPage,
                query.NormalizedPageSize);

        var cacheKey =
            $"vocabulary:public:{generation}:list:" +
            Hash(rawCacheKey);

        var cached =
            await _cache.GetAsync<
                PagedResult<VocabularyListItemResponse>>(
                cacheKey,
                cancellationToken);

        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var source =
            _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.Status ==
                        ContentStatus.Published)
                .Include(x => x.HskLevel)
                .Include(x => x.PartOfSpeech)
                .Include(x => x.Topic)
                .Include(x => x.AudioAsset)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Q))
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
                            x.PinyinNormalized,
                            $"%{keyword}%") ||
                        EF.Functions.ILike(
                            x.PrimaryMeaningVi,
                            $"%{keyword}%"));
        }

        if (query.HskLevel.HasValue)
        {
            source =
                source.Where(
                    x =>
                        x.HskLevelId ==
                        query.HskLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                query.Topic))
        {
            var topic =
                query.Topic.Trim()
                    .ToLowerInvariant();

            source =
                source.Where(
                    x =>
                        x.Topic != null &&
                        x.Topic.Slug == topic);
        }

        if (!string.IsNullOrWhiteSpace(
                query.PartOfSpeech))
        {
            var part =
                query.PartOfSpeech.Trim()
                    .ToLowerInvariant();

            source =
                source.Where(
                    x =>
                        x.PartOfSpeech != null &&
                        x.PartOfSpeech.Code == part);
        }

        if (query.Difficulty.HasValue)
        {
            source =
                source.Where(
                    x =>
                        x.Difficulty ==
                        query.Difficulty.Value);
        }

        source =
            ApplyPublicSort(
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
                .Take(query.NormalizedPageSize)
                .ToArrayAsync(
                    cancellationToken);

        var items =
            entities
                .Select(
                    VocabularyMapper
                        .ToPublicListItem)
                .ToArray();

        var response =
            new PagedResult<
                VocabularyListItemResponse>(
                items,
                query.NormalizedPage,
                query.NormalizedPageSize,
                total);

        await _cache.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(
                _options.ListMinutes),
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<VocabularyDetailResponse>>
        GetVocabularyAsync(
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(
                simplified))
        {
            return Result.Failure<
                VocabularyDetailResponse>(
                Error.Validation(
                    "Vocabulary.InvalidSimplified",
                    "Simplified không hợp lệ."));
        }

        simplified =
            simplified.Trim();

        pinyinNormalized =
            string.IsNullOrWhiteSpace(
                pinyinNormalized)
                ? null
                : pinyinNormalized.Trim();

        var generation =
            await GetGenerationAsync(
                cancellationToken);

        var cacheKey =
            $"vocabulary:public:{generation}:detail:" +
            Hash(
                $"{simplified}|{pinyinNormalized}");

        var cached =
            await _cache.GetAsync<
                VocabularyDetailResponse>(
                cacheKey,
                cancellationToken);

        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var query =
            _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.Status ==
                            ContentStatus.Published &&
                        x.Simplified ==
                            simplified);

        if (pinyinNormalized is not null)
        {
            query =
                query.Where(
                    x =>
                        x.PinyinNormalized ==
                        pinyinNormalized);
        }

        var matches =
            await query
                .Include(x => x.HskLevel)
                .Include(x => x.PartOfSpeech)
                .Include(x => x.Topic)
                .Include(x => x.AudioAsset)
                .Include(x => x.Meanings)
                .Include(x => x.Examples)
                    .ThenInclude(x => x.AudioAsset)
                .Take(2)
                .ToArrayAsync(
                    cancellationToken);

        if (matches.Length == 0)
        {
            return Result.Failure<
                VocabularyDetailResponse>(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy vocabulary."));
        }

        if (matches.Length > 1 &&
            pinyinNormalized is null)
        {
            return Result.Failure<
                VocabularyDetailResponse>(
                Error.Conflict(
                    "Vocabulary.Ambiguous",
                    "Có nhiều vocabulary cùng chữ. Hãy cung cấp pinyinNormalized."));
        }

        var entity =
            matches[0];

        var relations =
            await _db.Set<VocabularyRelation>()
                .AsNoTracking()
                .Include(
                    x => x.RelatedVocabulary)
                .Where(
                    x =>
                        x.VocabularyId ==
                            entity.Id &&
                        x.RelatedVocabulary.Status ==
                            ContentStatus.Published)
                .ToArrayAsync(
                    cancellationToken);

        var response =
            VocabularyMapper.ToPublicDetail(
                entity,
                relations);

        await _cache.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(
                _options.DetailMinutes),
            cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<
        IReadOnlyCollection<PublicTopicResponse>>>
        GetTopicsAsync(
            CancellationToken cancellationToken = default)
    {
        var generation =
            await GetGenerationAsync(
                cancellationToken);

        var key =
            $"vocabulary:public:{generation}:topics";

        var cached =
            await _cache.GetAsync<
                IReadOnlyCollection<
                    PublicTopicResponse>>(
                key,
                cancellationToken);

        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var entities =
            await _db.Set<Topic>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.Status ==
                        ContentStatus.Published)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.NameVi)
                .ToArrayAsync(
                    cancellationToken);

        var response =
            entities
                .Select(
                    VocabularyMapper
                        .ToPublicTopic)
                .ToArray();

        await _cache.SetAsync(
            key,
            (IReadOnlyCollection<
                PublicTopicResponse>)response,
            TimeSpan.FromMinutes(
                _options.TaxonomyMinutes),
            cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                PublicTopicResponse>>(
            response);
    }

    public async Task<Result<
        IReadOnlyCollection<
            PublicPartOfSpeechResponse>>>
        GetPartsOfSpeechAsync(
            CancellationToken cancellationToken = default)
    {
        var entities =
            await _db.Set<PartOfSpeech>()
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                PublicPartOfSpeechResponse>>(
            entities
                .Select(
                    VocabularyMapper
                        .ToPublicPartOfSpeech)
                .ToArray());
    }

    public async Task<Result<
        IReadOnlyCollection<PublicHskLevelResponse>>>
        GetHskLevelsAsync(
            CancellationToken cancellationToken = default)
    {
        var entities =
            await _db.Set<HskLevel>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<
                PublicHskLevelResponse>>(
            entities
                .Select(
                    VocabularyMapper.ToPublicHsk)
                .ToArray());
    }

    public async Task<Result<VocabularyNoteResponse?>>
        GetMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default)
    {
        var vocabularyResult =
            await ResolveVocabularyAsync(
                simplified,
                pinyinNormalized,
                cancellationToken);

        if (vocabularyResult.IsFailure)
        {
            return Result.Failure<
                VocabularyNoteResponse?>(
                vocabularyResult.Error);
        }

        var note =
            await _db.Set<UserVocabularyNote>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            vocabularyResult.Value.Id,
                    cancellationToken);

        return Result.Success<
            VocabularyNoteResponse?>(
            note is null
                ? null
                : VocabularyMapper
                    .ToNoteResponse(note));
    }

    public async Task<Result<VocabularyNoteResponse>>
        SaveMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            SaveVocabularyNoteRequest request,
            CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<
                VocabularyNoteResponse>(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Người dùng chưa đăng nhập."));
        }

        var vocabularyResult =
            await ResolveVocabularyAsync(
                simplified,
                pinyinNormalized,
                cancellationToken);

        if (vocabularyResult.IsFailure)
        {
            return Result.Failure<
                VocabularyNoteResponse>(
                vocabularyResult.Error);
        }

        var vocabulary =
            vocabularyResult.Value;

        var note =
            await _db.Set<UserVocabularyNote>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            vocabulary.Id,
                    cancellationToken);

        if (note is null)
        {
            note =
                new UserVocabularyNote(
                    userId,
                    vocabulary.Id,
                    request.Content);

            note.SetPinned(
                request.IsPinned);

            _db.Set<UserVocabularyNote>()
                .Add(note);
        }
        else
        {
            note.UpdateContent(
                request.Content);

            note.SetPinned(
                request.IsPinned);
        }

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            VocabularyMapper
                .ToNoteResponse(note));
    }

    public async Task<Result>
        DeleteMyNoteAsync(
            Guid userId,
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken = default)
    {
        var vocabularyResult =
            await ResolveVocabularyAsync(
                simplified,
                pinyinNormalized,
                cancellationToken);

        if (vocabularyResult.IsFailure)
        {
            return Result.Failure(
                vocabularyResult.Error);
        }

        var note =
            await _db.Set<UserVocabularyNote>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            vocabularyResult.Value.Id,
                    cancellationToken);

        if (note is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "VocabularyNote.NotFound",
                    "Không tìm thấy ghi chú."));
        }

        _db.Remove(note);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result<
        Domain.Entities.Vocabulary.Vocabulary>>
        ResolveVocabularyAsync(
            string simplified,
            string? pinyinNormalized,
            CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                simplified))
        {
            return Result.Failure<
                Domain.Entities.Vocabulary.Vocabulary>(
                Error.Validation(
                    "Vocabulary.InvalidSimplified",
                    "Simplified không hợp lệ."));
        }

        simplified =
            simplified.Trim();

        var query =
            _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.Status ==
                            ContentStatus.Published &&
                        x.Simplified ==
                            simplified);

        if (!string.IsNullOrWhiteSpace(
                pinyinNormalized))
        {
            var normalized =
                pinyinNormalized.Trim();

            query =
                query.Where(
                    x =>
                        x.PinyinNormalized ==
                        normalized);
        }

        var values =
            await query
                .Take(2)
                .ToArrayAsync(
                    cancellationToken);

        if (values.Length == 0)
        {
            return Result.Failure<
                Domain.Entities.Vocabulary.Vocabulary>(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy vocabulary."));
        }

        if (values.Length > 1 &&
            string.IsNullOrWhiteSpace(
                pinyinNormalized))
        {
            return Result.Failure<
                Domain.Entities.Vocabulary.Vocabulary>(
                Error.Conflict(
                    "Vocabulary.Ambiguous",
                    "Có nhiều vocabulary cùng chữ."));
        }

        return Result.Success(values[0]);
    }

    private async Task<string>
        GetGenerationAsync(
            CancellationToken cancellationToken)
    {
        var generation =
            await _cache.GetAsync<string>(
                GenerationCacheKey,
                cancellationToken);

        if (!string.IsNullOrWhiteSpace(
                generation))
        {
            return generation;
        }

        generation =
            Guid.NewGuid()
                .ToString("N");

        await _cache.SetAsync(
            GenerationCacheKey,
            generation,
            TimeSpan.FromDays(
                _options.GenerationDays),
            cancellationToken);

        return generation;
    }

    private static string Hash(
        string value)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    value));

        return Convert.ToHexString(
                bytes);
    }

    private static IQueryable<
        Domain.Entities.Vocabulary.Vocabulary>
        ApplyPublicSort(
            IQueryable<
                Domain.Entities.Vocabulary.Vocabulary> source,
            string? sort)
    {
        return sort?
            .Trim()
            .ToLowerInvariant()
            switch
        {
            "-simplified" =>
                source.OrderByDescending(
                    x => x.Simplified),

            "pinyin" =>
                source.OrderBy(
                    x => x.PinyinNormalized),

            "-pinyin" =>
                source.OrderByDescending(
                    x => x.PinyinNormalized),

            "hsk" =>
                source
                    .OrderBy(
                        x => x.HskLevelId)
                    .ThenBy(
                        x => x.Simplified),

            "-hsk" =>
                source
                    .OrderByDescending(
                        x => x.HskLevelId)
                    .ThenBy(
                        x => x.Simplified),

            "difficulty" =>
                source
                    .OrderBy(
                        x => x.Difficulty),

            "-difficulty" =>
                source
                    .OrderByDescending(
                        x => x.Difficulty),

            _ =>
                source.OrderBy(
                    x => x.Simplified)
        };
    }
}
