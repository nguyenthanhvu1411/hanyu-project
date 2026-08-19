using HanYu.Application.Features.Vocabulary.Admin.Vocabulary;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Vocabulary;

public static class VocabularyWorkflowValidator
{
    public static async Task<VocabularyValidationResultDto?> ValidateAsync(
        HanYuDbContext db,
        long vocabularyId,
        bool forPublish,
        CancellationToken cancellationToken = default)
    {
        var vocabulary = await db.Set<Domain.Entities.Vocabulary.Vocabulary>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vocabularyId, cancellationToken);

        if (vocabulary is null)
            return null;

        var issues = new List<VocabularyValidationIssueDto>();

        AddRequired(issues, vocabulary.Simplified, "Vocabulary.SimplifiedRequired", "Chữ giản thể là bắt buộc.", "simplified");
        AddRequired(issues, vocabulary.Pinyin, "Vocabulary.PinyinRequired", "Pinyin là bắt buộc.", "pinyin");
        AddRequired(issues, vocabulary.PinyinNormalized, "Vocabulary.PinyinNormalizedRequired", "Pinyin normalized là bắt buộc.", "pinyinNormalized");
        AddRequired(issues, vocabulary.PrimaryMeaningVi, "Vocabulary.PrimaryMeaningRequired", "Nghĩa chính tiếng Việt là bắt buộc.", "primaryMeaningVi");

        var hskValid = await db.Set<HskLevel>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == vocabulary.HskLevelId && x.IsActive, cancellationToken);
        if (!hskValid)
        {
            issues.Add(Error("Vocabulary.InvalidHsk", "Cấp độ HSK không tồn tại hoặc đang bị vô hiệu hóa.", "hskLevelId"));
        }

        if (vocabulary.PartOfSpeechId.HasValue)
        {
            var partExists = await db.Set<PartOfSpeech>()
                .AsNoTracking()
                .AnyAsync(x => x.Id == vocabulary.PartOfSpeechId.Value, cancellationToken);
            if (!partExists)
                issues.Add(Error("Vocabulary.InvalidPartOfSpeech", "Từ loại đã chọn không còn tồn tại.", "partOfSpeechId"));
        }

        if (vocabulary.TopicId.HasValue)
        {
            var topic = await db.Set<Topic>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vocabulary.TopicId.Value, cancellationToken);

            if (topic is null)
            {
                issues.Add(Error("Vocabulary.InvalidTopic", "Chủ đề đã chọn không còn tồn tại.", "topicId"));
            }
            else if (topic.Status != ContentStatus.Published)
            {
                issues.Add(forPublish
                    ? Error("Vocabulary.TopicNotPublished", "Chủ đề phải được Published trước khi xuất bản từ vựng.", "topicId")
                    : Warning("Vocabulary.TopicNotPublished", "Chủ đề chưa Published; cần xử lý trước bước xuất bản.", "topicId"));
            }
        }

        var meanings = await db.Set<VocabularyMeaning>()
            .AsNoTracking()
            .Where(x => x.VocabularyId == vocabularyId)
            .OrderBy(x => x.SenseOrder)
            .ToArrayAsync(cancellationToken);

        if (meanings.Length == 0)
        {
            issues.Add(Error("Vocabulary.MeaningRequired", "Từ vựng phải có ít nhất một nghĩa trước khi gửi duyệt.", "meanings"));
        }
        else
        {
            if (meanings.Any(x => string.IsNullOrWhiteSpace(x.MeaningVi)))
                issues.Add(Error("Vocabulary.MeaningEmpty", "Có nghĩa từ vựng đang để trống.", "meanings"));

            var duplicateOrder = meanings
                .GroupBy(x => x.SenseOrder)
                .Any(group => group.Count() > 1);
            if (duplicateOrder)
                issues.Add(Error("Vocabulary.MeaningDuplicateOrder", "SenseOrder của nghĩa từ vựng bị trùng.", "meanings"));
        }

        var examples = await db.Set<VocabularyExample>()
            .AsNoTracking()
            .Where(x => x.VocabularyId == vocabularyId)
            .ToArrayAsync(cancellationToken);

        if (examples.Length == 0)
        {
            issues.Add(forPublish
                ? Error("Vocabulary.ExampleRequired", "Từ vựng phải có ít nhất một câu ví dụ trước khi xuất bản.", "examples")
                : Warning("Vocabulary.ExampleRequired", "Nên bổ sung ít nhất một câu ví dụ trước khi xuất bản.", "examples"));
        }
        else if (forPublish && examples.Any(x => x.Status != ContentStatus.Published))
        {
            issues.Add(Error("Vocabulary.ExampleNotPublished", "Tất cả câu ví dụ của từ vựng phải được Published trước khi xuất bản từ vựng.", "examples"));
        }

        if (!vocabulary.AudioAssetId.HasValue)
        {
            issues.Add(forPublish
                ? Error("Vocabulary.AudioRequired", "Từ vựng phải có audio phát âm trước khi xuất bản.", "audioAssetId")
                : Warning("Vocabulary.AudioRequired", "Chưa có audio phát âm; cần bổ sung trước khi xuất bản.", "audioAssetId"));
        }
        else
        {
            var audio = await db.Set<AudioAsset>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vocabulary.AudioAssetId.Value, cancellationToken);

            if (audio is null)
            {
                issues.Add(Error("Vocabulary.AudioNotFound", "AudioAsset đang gắn không còn tồn tại.", "audioAssetId"));
            }
            else
            {
                if (audio.Kind != AudioAssetKind.Vocabulary)
                    issues.Add(Error("Vocabulary.InvalidAudioKind", "Audio gắn cho từ vựng phải có loại Vocabulary.", "audioAssetId"));

                if (audio.Status == ContentStatus.Archived)
                    issues.Add(Error("Vocabulary.AudioArchived", "Audio phát âm đã bị lưu trữ.", "audioAssetId"));

                if (string.IsNullOrWhiteSpace(audio.StoragePath) || !audio.MimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                    issues.Add(Error("Vocabulary.AudioInvalidFile", "Audio phát âm chưa có file hoặc MIME type không hợp lệ.", "audioAssetId"));

                if (forPublish && audio.Status != ContentStatus.Published)
                    issues.Add(Error("Vocabulary.AudioNotPublished", "Audio phát âm phải được Published trước khi xuất bản từ vựng.", "audioAssetId"));
            }
        }

        if (forPublish && vocabulary.Status != ContentStatus.Approved)
        {
            issues.Add(Error("Vocabulary.NotApproved", "Vocabulary phải ở trạng thái Approved trước khi Publish.", "status"));
        }

        return new VocabularyValidationResultDto(
            !issues.Any(issue => issue.Severity == VocabularyValidationSeverity.Error),
            issues);
    }

    private static void AddRequired(
        ICollection<VocabularyValidationIssueDto> issues,
        string? value,
        string code,
        string message,
        string field)
    {
        if (string.IsNullOrWhiteSpace(value))
            issues.Add(Error(code, message, field));
    }

    private static VocabularyValidationIssueDto Error(string code, string message, string? field = null)
        => new(code, message, field, VocabularyValidationSeverity.Error);

    private static VocabularyValidationIssueDto Warning(string code, string message, string? field = null)
        => new(code, message, field, VocabularyValidationSeverity.Warning);
}
