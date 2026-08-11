using System.Text;
using HanYu.Application.Interfaces.Storage;
using HanYu.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CourseEntity = HanYu.Domain.Entities.Course.Course;

namespace HanYu.Infrastructure.Persistence.Seeding.Content;

public sealed class CourseContentSeeder
{
    private const string StorageReferencePrefix = "storage://";

    private readonly HanYuDbContext _db;
    private readonly IPublicFileStorage _storage;
    private readonly ContentSeedOptions _options;
    private readonly StorageOptions _storageOptions;
    private readonly ILogger<CourseContentSeeder> _logger;

    public CourseContentSeeder(
        HanYuDbContext db,
        IPublicFileStorage storage,
        IOptions<ContentSeedOptions> options,
        IOptions<StorageOptions> storageOptions,
        ILogger<CourseContentSeeder> logger)
    {
        _db = db;
        _storage = storage;
        _options = options.Value;
        _storageOptions = storageOptions.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(_storageOptions.PublicBucketName))
        {
            _logger.LogWarning(
                "Content seed skipped because Storage:PublicBucketName is not configured.");
            return;
        }

        var hskLevels = await _db.HskLevels
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var definition in Definitions)
        {
            if (await _db.Courses.AnyAsync(
                    x => x.Code == definition.Code,
                    cancellationToken))
            {
                continue;
            }

            if (!hskLevels.TryGetValue(definition.HskCode, out var hskLevel))
            {
                _logger.LogWarning(
                    "Course seed {CourseCode} skipped because {HskCode} does not exist.",
                    definition.Code,
                    definition.HskCode);
                continue;
            }

            var objectKey = $"seed/course-covers/{definition.Code.ToLowerInvariant()}.svg";
            var svgBytes = Encoding.UTF8.GetBytes(BuildCoverSvg(definition));

            await using var stream = new MemoryStream(svgBytes, writable: false);
            var uploaded = await _storage.UploadAsync(
                objectKey,
                stream,
                "image/svg+xml",
                cancellationToken);

            try
            {
                var course = new CourseEntity(
                    definition.Code,
                    definition.Slug,
                    definition.Title,
                    hskLevel.Id,
                    definition.SortOrder,
                    definition.ShortDescription,
                    definition.Description,
                    $"{StorageReferencePrefix}{uploaded.ObjectKey}",
                    definition.EstimatedMinutes);

                _db.Courses.Add(course);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Seeded course {CourseCode} with cover stored at {ObjectKey}.",
                    definition.Code,
                    uploaded.ObjectKey);
            }
            catch
            {
                await _storage.DeleteAsync(uploaded.ObjectKey, cancellationToken);
                throw;
            }
        }
    }

    private static string BuildCoverSvg(CourseSeedDefinition definition)
    {
        var safeTitle = System.Net.WebUtility.HtmlEncode(definition.Title);
        var safeCode = System.Net.WebUtility.HtmlEncode(definition.HskCode);

        return $$"""
        <svg xmlns="http://www.w3.org/2000/svg" width="1200" height="675" viewBox="0 0 1200 675">
          <defs>
            <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
              <stop offset="0" stop-color="{{definition.ColorFrom}}"/>
              <stop offset="1" stop-color="{{definition.ColorTo}}"/>
            </linearGradient>
          </defs>
          <rect width="1200" height="675" rx="36" fill="url(#bg)"/>
          <circle cx="1010" cy="130" r="210" fill="#ffffff" opacity="0.10"/>
          <circle cx="1110" cy="590" r="260" fill="#ffffff" opacity="0.08"/>
          <text x="84" y="170" fill="#ffffff" font-size="72" font-family="Arial, sans-serif" font-weight="700">{{safeCode}}</text>
          <text x="84" y="278" fill="#ffffff" font-size="48" font-family="Arial, sans-serif" font-weight="700">{{safeTitle}}</text>
          <text x="84" y="355" fill="#ffffff" opacity="0.88" font-size="28" font-family="Arial, sans-serif">Học tiếng Trung theo lộ trình HanYu</text>
          <rect x="84" y="470" width="310" height="72" rx="36" fill="#ffffff" opacity="0.18"/>
          <text x="124" y="518" fill="#ffffff" font-size="26" font-family="Arial, sans-serif" font-weight="600">课程 · COURSE</text>
        </svg>
        """;
    }

    private sealed record CourseSeedDefinition(
        string Code,
        string HskCode,
        string Slug,
        string Title,
        string ShortDescription,
        string Description,
        int SortOrder,
        int EstimatedMinutes,
        string ColorFrom,
        string ColorTo);

    private static readonly CourseSeedDefinition[] Definitions =
    [
        new("COURSE-HSK1", "HSK1", "hsk-1-nen-tang-tieng-trung", "HSK 1 - Nền tảng tiếng Trung", "Làm quen phát âm, chữ Hán và các mẫu câu giao tiếp cơ bản.", "Khóa học xây dựng nền tảng tiếng Trung cho người mới bắt đầu: Pinyin, thanh điệu, chữ Hán cơ bản, từ vựng HSK 1 và hội thoại hằng ngày.", 1, 360, "#ef241c", "#ff766f"),
        new("COURSE-HSK2", "HSK2", "hsk-2-giao-tiep-co-ban", "HSK 2 - Giao tiếp cơ bản", "Mở rộng từ vựng và phản xạ trong các tình huống quen thuộc.", "Khóa học tập trung vào giao tiếp đời sống, cấu trúc câu HSK 2 và khả năng nghe đọc những đoạn hội thoại ngắn.", 2, 480, "#f97316", "#fb923c"),
        new("COURSE-HSK3", "HSK3", "hsk-3-trung-cap", "HSK 3 - Tiếng Trung trung cấp", "Phát triển đồng đều nghe, nói, đọc và vốn từ trung cấp.", "Lộ trình HSK 3 giúp người học sử dụng tiếng Trung độc lập hơn trong học tập, công việc và các chủ đề đời sống thường gặp.", 3, 600, "#eab308", "#facc15"),
        new("COURSE-HSK4", "HSK4", "hsk-4-ung-dung-thuc-te", "HSK 4 - Ứng dụng thực tế", "Nâng khả năng diễn đạt, đọc hiểu và xử lý hội thoại dài hơn.", "Khóa học HSK 4 tập trung vào diễn đạt tự nhiên, đọc hiểu văn bản trung bình và sử dụng ngữ pháp trong tình huống thực tế.", 4, 720, "#16a34a", "#4ade80"),
        new("COURSE-HSK5", "HSK5", "hsk-5-nang-cao", "HSK 5 - Tiếng Trung nâng cao", "Mở rộng khả năng đọc, viết và sử dụng từ vựng học thuật.", "Lộ trình HSK 5 dành cho người học muốn đọc hiểu nội dung dài, diễn đạt ý kiến phức tạp và chuẩn bị cho môi trường học tập hoặc công việc bằng tiếng Trung.", 5, 900, "#2563eb", "#60a5fa"),
        new("COURSE-HSK6", "HSK6", "hsk-6-chuyen-sau", "HSK 6 - Tiếng Trung chuyên sâu", "Rèn khả năng hiểu và diễn đạt tiếng Trung ở mức độ chuyên sâu.", "Khóa học HSK 6 phát triển năng lực đọc hiểu, nghe hiểu và diễn đạt chính xác với các chủ đề phức tạp, chuẩn bị cho học thuật và công việc chuyên môn.", 6, 1080, "#7c3aed", "#a78bfa")
    ];
}
